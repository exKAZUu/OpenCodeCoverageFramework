#include <stdio.h>
#define N 9

int quicksort( int data[], int left, int right ){
    int pl, pr, pivot, tmp;
    pl = left; pr = right;
    pivot = data[ (pl+pr)/2 ];

    printf( "---\nquicksort( data, %d, %d )\n", left, right );
    while( pl <= pr ){
        while( data[pl] < pivot ){ pl++; }
        printf( " ** found data[%d]=%d >= %d\n", pl, data[pl], pivot );
        while( pivot < data[pr] ){ pr--; }
        printf( " ** found data[%d]=%d <= %d\n", pr, data[pr], pivot );
        
        if( pl <= pr ){
            printf( " swap: data[%d]=%d <=> data[%d]=%d\n",
                    pl, data[pl], pr, data[pr] );
            tmp = data[pl];
            data[pl] = data[pr];
            data[pr] = tmp;
            pl++;
            pr--;
        }
    }
    if( left < pr ){
        quicksort( data, left, pr );
    }
    if( pl < right ){
        quicksort( data, pl, right );
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
    
    quicksort( data, 0, 8 );

    printf( "\n== after ==\n" );
    for( i=0; i<N; i++ ){
        printf( "%d ", data[i] );
    }
    printf( "\n" );

    return 0;
}
