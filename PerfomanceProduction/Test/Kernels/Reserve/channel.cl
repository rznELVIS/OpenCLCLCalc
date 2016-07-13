__kernel void channel(__global int* source)
{
	int gid = get_global_id(0);

	int c = UINT_MAX;

	source[0] = 1;// | source[gid];
	source[5] = 1;// | source[gid];
	source[8] = 1;// | source[gid];
	source[11] = 0;// | source[gid];
	//source[22] = 1;
}