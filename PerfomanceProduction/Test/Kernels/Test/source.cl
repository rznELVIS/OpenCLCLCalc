// заполнение исходного массива данных.
//	source- исходный массив данных.
__kernel void source(__global int* source, __global const int* seed)
{
	const uint gid = get_global_id(0);
	const uint lid = get_global_id(0);

	uint start = gid *[%k%];

	uint* s[2];
	s[0] = gid * [%random%] + 4 * lid;
	s[1] = seed[0] * [%random%] + lid;

	for (int i = 0; i < [%k%]; i++)
	{
		uint x = s[0];
		uint y = s[1];

		s[0] = y;
		x ^= x << 23;
		s[1] = x ^ y ^ (x >> 17) ^ (y >> 26);

		int res = s[1] + y;
		source[gid * [%k%] + i] = (res & 1); // проверка четная или нечетная
	}
}
