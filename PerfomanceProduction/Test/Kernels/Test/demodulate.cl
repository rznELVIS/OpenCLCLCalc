// демодуляция - BPSK.
__kernel void demodulate(__global int* channelled)
{
	int gid = get_global_id(0);
	
	channelled[gid] = (channelled[gid] + 1) / 2;
}