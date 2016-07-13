// первое заполнение синдрома.
__kernel void fillSyndrom(__global const int* coded, __global int* syndrom)
{
	const int gid = get_global_id(0);

	syndrom[gid] = coded[gid + [%start%]];
}