__kernel void reset(__global int* data)
{
	int gid = get_global_id(0);

	data[gid] =  0;
}