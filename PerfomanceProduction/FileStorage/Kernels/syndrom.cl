// заполнение синдрома.
__kernel void syndrom(__global const int* coded, __global int* syndrom)
{
	const int gid = get_global_id(0);

	[%adders%];

	const int pointer = gid % [%branch%];
	const int iteration = gid / [%branch%];
	const int start = iteration * [%k%] + [%v%] * [%branch%];
	for(int i = 0; i < [%adderCount%]; i++)
	{
		const int index = ([%branch%] - pointer + adders[i]) % [%branch%] + [%u%] * [%branch%] + iteration *[%k%];
		syndrom[start + pointer] = syndrom[start + pointer] ^ coded[index];
	}
}