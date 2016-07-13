__kernel void code(__global const int* source, __global int* coded)
{
	const int gid = get_global_id(0);
	const int u = get_global_id(1);
	const int v = get_global_id(2);

	/*[%adders%];

	const int pointer = gid % [%branch%];
	const int iteration = gid / [%branch%];
	const int start = [%start%] + iteration *[%k%] + u *[%branch%];
	for(int i = 0; i < [%adderCount%]; i++)
	{
		const int index = ([%branch%] - pointer + adders[i]) % [%branch%] + u * [%branch%] + iteration * [%k%];
		coded[start + pointer] = coded[start + pointer] ^ source[index];
	}*/

	coded[gid] = (u < coded[gid]) ? -1 : u;
}