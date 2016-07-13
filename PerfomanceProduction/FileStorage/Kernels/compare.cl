// сравнение двух массивов.
//	source- исходный массив данных.
//	coded - массив данных после операций кодирования и декодирования.
//	result - результирующий массив (хранит несовпадающие биты.).
__kernel void compare(__global const int* source, __global const int* coded, __global int* result)
{
	const int gid = get_global_id(0);

	atomic_add(&result[0], (source[gid] ^ coded[gid]));
}
