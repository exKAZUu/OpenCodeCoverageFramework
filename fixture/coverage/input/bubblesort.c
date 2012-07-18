#include <stdio.h>
#define N 9

int bubblesort( int data[] ){
    int i, j, tmp;

    for( i=1; i<=N-1; i++ ){
        printf( "---\n%d-th pass\n", i );
        for( j=0; j<N-i; j++ ){
            printf( "[%d %d] => ", data[j], data[j+1] );
            if( data[j] > data[j+1] ){
                tmp = data[j];
                data[j] = data[j+1];
                data[j+1] = tmp;
                printf( "[%d %d]\n", data[j], data[j+1] );
            } else {
                printf( "OK\n" );
            }
        }
    }

    return 0;
}

int main(){
    int data[N] = { 5, 7, 1, 4, 6, 2, 3, 9, 8 };
    int i;

    printf( "== before ==\n" );
    for( i=0; i<N; i++ ){
        printf( "%d ", data[i] );
    }
    printf( "\n\n" );
    
    bubblesort( data );

    printf( "\n== after ==\n" );
    for( i=0; i<N; i++ ){
        printf( "%d ", data[i] );
    }
    printf( "\n" );

    return 0;
}
