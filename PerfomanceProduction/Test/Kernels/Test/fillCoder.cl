// заполнение информационной части кодового слова.
//	source - исходное сообщение
//	coded - закодированное сообщение.
__kernel void fillCoder(__global const int* source, __global int* coded)
{
	const int gid = get_global_id(0);

	coded[gid] = source[gid];
}