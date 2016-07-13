// модуляция - BPSK.
__kernel void modulate(__global int* coded)
{
	int gid = get_global_id(0);
	
	coded[gid] = 2 * coded[gid] - 1;
}