// передача значения по каналу.
__kernel void channel(__global int* modulated, __global int* seed)
{
	uint gid = get_global_id(0);
	uint lid = get_global_id(0);

	uint start = gid * [%n%];

	uint* s[2];
	s[0] = gid * 2 + lid;
	s[1] = seed[0] + 3 * lid;

	uint x, y, res;
	float r1, r2, value;
	for (int i = 0; i < [%n%]; i++)
	{
		x = s[0];
		y = s[1];
		s[0] = y;
		x ^= x << 23;
		s[1] = x ^ y ^ (x >> 17) ^ (y >> 26);
		res = s[1] + y;
		r1 = ((float)abs(res)) / UINT_MAX;
		
		x = s[0];
		y = s[1];
		s[0] = y;
		x ^= x << 23;
		s[1] = x ^ y ^ (x >> 17) ^ (y >> 26);
		res = s[1] + y;
		r2 = ((float)abs(res)) / UINT_MAX;
		
		value = sqrt((-2 * log10(r1))) * cos(2 * [%PI%] * r2);
		value = [%M%] + value * sqrt([%sigma%]);

		value = value + (modulated[gid * [%n%] + i] == -1 ? -1.0f : 1.0f);

		modulated[gid * [%n%] + i] = value > 0 ? 1 : -1;
	}
}