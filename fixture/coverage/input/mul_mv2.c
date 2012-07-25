#include <stdio.h>
#include <stdlib.h>


int main(void)
{
    int i, j, n;
    double *a, *ai, *x, *y;


    /*  input dimension n  */
    printf("Dimension : n = ");
    scanf("%d", &n);


    /*  allocate memory for A, x, and y  */
    a = (double *)malloc(n*n*sizeof(double));
    if (a==NULL) {
        printf("Can't allocate memory.\n");
        exit(1);
    }
    x = (double *)malloc(n*sizeof(double));
    if (x==NULL) {
        printf("Can't allocate memory.\n");
        exit(1);
    }
    y = (double *)malloc(n*sizeof(double));
    if (y==NULL) {
        printf("Can't allocate memory.\n");
        exit(1);
    }


    /*  input data of matrix A  */
    printf("\n");
    ai = a;
    for (i=0; i<n; i++) {
        for(j=0; j<n; j++) {
            printf("a[%d][%d] = ", i, j);
            scanf("%lf", ai + j);
        }
        ai += n;
    }


    /*  input data of vector x  */
    printf("\n");
    for (i=0; i<n; i++) {
        printf("x[%d] = ", i);
        scanf("%lf", x + i);
    }


    /*  print matrix A  */
    printf("\n  a =\n");
    ai = a;
    for (i=0; i<n; i++) {
        for(j=0; j<n; j++) {
            printf("    %.2f", *(ai + j));
        }
        ai += n;
        printf("\n");
    }


    /*  print vector x  */
    printf("\n  x =\n");
    for (i=0; i<n; i++) {
        printf("    %.2f\n", *(x + i));
    }


    /*  calculate y = A*x  */
    ai = a;
    for (i=0; i<n; i++) {
        *(y + i) = 0.0;
        for (j=0; j<n; j++) {
            *(y + i) += *(ai + j) * *(x + j);
        }
        ai += n;
    }


    /*  print answer  */
    printf("\n  a*x = \n");
    for (i=0; i<n; i++) {
        printf("    %.2f\n", *(y + i));
    }


    /*  free memory  */
    free(a);
    free(x);
    free(y);


    return 0;
}
