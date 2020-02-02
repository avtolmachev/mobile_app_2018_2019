#include "ReadWriter.h"
#include "MergeSort.h"
//iostream, fstream включены в ReadWriter.h

//Не рекомендуется добавлять собственные вспомогательные классы и методы.
//Необходимо использовать уже имеющиеся классы и методы, добавив реализацию, соответствующую описанию.
using namespace std;

//Описание методов на английском языке имеется в классе MergeSort, в файле MergeSort.h
void MergeSort::sort(int *arr, int length)
{
    MergeSort::divide_and_merge(arr, 0, length);
}

void MergeSort::merge(int* arr, int first, int second, int end)
{
    int length = end - first + 1;
    int *b = new int[length];
    int old_second = second;
    int old_first = first;

    for (int i = 0; i < length; ++i)
    {
        if (first == old_second)
        {
            b[i] = arr[second];
            ++second;
            continue;
        }
        else if (second == end)
        {
            b[i] = arr[first];
            ++first;
            continue;
        }
        else
        {
            if (arr[first] < arr[second])
            {
                b[i] = arr[first];
                ++first;
            }
            else
            {
                b[i] = arr[second];
                ++second;
            }
        }
    }

    int j = 0;
    for (int i = old_first; i < end; ++i)
        {
            arr[i] = b[j];
            ++j;
        }
    delete[] b;
}

void MergeSort::divide_and_merge(int *arr, int left, int right)
{
    if (left < right - 1)
    {
        int m = (left + right) / 2;
        divide_and_merge(arr, left, m);
        divide_and_merge(arr, m, right);
        merge(arr, left, m, right);
    }
}

int main()
{
    ReadWriter rw;

    int *brr = nullptr;
    int length;

    //Read data from file
    length = rw.readInt();

    brr = new int[length];
    rw.readArray(brr, length);

    //Start sorting
    MergeSort s;

    s.sort(brr, length);

    //Write answer to file
    rw.writeArray(brr, length);

    delete[] brr;

    return 0;
}
