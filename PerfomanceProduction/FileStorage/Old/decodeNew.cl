__kernel void decode(__global int* coded, __global int* syndrom, __global int* diffrence)
{
	int temp = get_global_id(0);

	uint gid = get_global_id(0);
	uint pointer = get_global_id(1);

	[%adders%];

	int thershold, index;

	const int start = gid* [%k%];

	for (int u = 0; u < [%u%]; u++) {
		int diffStart = gid*[%m%] + u *[%branch%];

		//вычисление значения на ПЭ.
		thershold = 0;
		for (int v = 0; v < [%v%]; v++)
		{
			for (int i = 0; i < [%adderLength%]; i++)
			{
				int adderIndex = i + [%adderLength%] * v + [%adderBranch%] * u;
				index = ([%branch%] - pointer + adders[adderIndex]) % [%branch%] + start + [%branch%] * v;
				thershold += syndrom[index];
			}
		}

		thershold += diffrence[gid *[%m%] + pointer + u *[%branch%]];
		if (thershold <= [%thershold%]) return;

		coded[gid *[%m%] + pointer + u *[%branch%]] = coded[gid *[%m%] + pointer + u *[%branch%]] ^ 1;
		diffrence[gid *[%m%] + pointer + u *[%branch%]] = diffrence[gid *[%m%] + pointer + u *[%branch%]] ^ 1;

		for (int v = 0; v < [%v%]; v++)
		{
			for (int i = 0; i < [%adderLength%]; i++)
			{
				int adderIndex = i + [%adderLength%] * v + [%adderBranch%] * u;
				index = ([%branch%] - pointer + adders[adderIndex]) % [%branch%] + start + [%branch%] * v;
				syndrom[index] = syndrom[index] ^ 1;
			}
		}
	}
}