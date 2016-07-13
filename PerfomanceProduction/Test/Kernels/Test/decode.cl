__kernel void decode(__global int* coded, __global int* syndrom, __global int* diffrence)
{
	const int gid = get_global_id(0);
	const int pointer = get_global_id(1);
	const int u = get_global_id(2);

	[%adders%];

	int thershold, index;

	const int start = gid * [%k%];
	const int diffStart = gid *[%m%] + u * [%branch%];

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

	if (thershold <= [%thershold%]) return;

	coded[gid *[%m%] + pointer + u * [%branch%]] = coded[gid * [%m%] + pointer + u * [%branch%]] ^ 1;
	diffrence[gid *[%m%] + pointer + u * [%branch%]] = diffrence[gid * [%m%] + pointer + u * [%branch%]] ^ 1;

	// коррекция синдрома.
	for (int v = 0; v <[%v%]; v++)
	{
		for (int i = 0; i <[%adderLength%]; i++)
		{
			int adderIndex = i + [%adderLength%] * v + [%adderBranch%] * u;
			index = ([%branch%] - pointer + adders[adderIndex]) % [%branch%] + start + [%branch%] * v;
			syndrom[index] = syndrom[index] ^ 1;
		}
	}
}