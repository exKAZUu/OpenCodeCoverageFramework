/* Definitions for bytecode */

#ifndef Py_CODE_H
#define Py_CODE_H
#ifdef __cplusplus
extern "C" {
#endif

/* Bytecode object */
typedef struct {
    PyObject_HEAD
    int co_argcount;		/* #arguments, except *args */
    int co_kwonlyargcount;	/* #keyword only arguments */
    int co_nlocals;		/* #local variables */
    int co_stacksize;		/* #entries needed for evaluation stack */
    int co_flags;		/* CO_..., see below */
    PyObject *co_code;		/* instruction opcodes */
    PyObject *co_consts;	/* list (constants used) */
    PyObject *co_names;		/* list of strings (names used) */
    PyObject *co_varnames;	/* tuple of strings (local variable names) */
    PyObject *co_freevars;	/* tuple of strings (free variable names) */
    PyObject *co_cellvars;      /* tuple of strings (cell variable names) */
    /* The rest doesn't count for hash or comparisons */
    PyObject *co_filename;	/* unicode (where it was loaded from) */
    PyObject *co_name;		/* unicode (name, for reference) */
    int co_firstlineno;		/* first source line number */
    PyObject *co_lnotab;	/* string (encoding addr<->lineno mapping) */
    void *co_zombieframe;     /* for optimization only (see frameobject.c) */
} PyCodeObject;

/* Masks for co_flags above */
#define CO_OPTIMIZED	0x0001
#define CO_NEWLOCALS	0x0002
#define CO_VARARGS	0x0004
#define CO_VARKEYWORDS	0x0008
#define CO_NESTED       0x0010
#define CO_GENERATOR    0x0020
/* The CO_NOFREE flag is set if there are no free or cell variables.
   This information is redundant, but it allows a single flag test
   to determine whether there is any extra work to be done when the
   call frame it setup.
*/
#define CO_NOFREE       0x0040

/* These are no longer used. */
#if 0
#define CO_GENERATOR_ALLOWED    0x1000
#endif
#define CO_FUTURE_DIVISION    	0x2000
#define CO_FUTURE_ABSOLUTE_IMPORT 0x4000 /* do absolute imports by default */
#define CO_FUTURE_WITH_STATEMENT  0x8000
#define CO_FUTURE_PRINT_FUNCTION  0x10000
#define CO_FUTURE_UNICODE_LITERALS 0x20000

#define CO_FUTURE_BARRY_AS_BDFL  0x40000

/* This should be defined if a future statement modifies the syntax.
   For example, when a keyword is added.
*/
#define PY_PARSER_REQUIRES_FUTURE_KEYWORD

#define CO_MAXBLOCKS 20 /* Max static block nesting within a function */

PyAPI_DATA(PyTypeObject) PyCode_Type;

#define PyCode_Check(op) (Py_TYPE(op) == &PyCode_Type)
#define PyCode_GetNumFree(op) (PyTuple_GET_SIZE((op)->co_freevars))

/* Public interface */
PyAPI_FUNC(PyCodeObject *) PyCode_New(
	int, int, int, int, int, PyObject *, PyObject *,
	PyObject *, PyObject *, PyObject *, PyObject *,
	PyObject *, PyObject *, int, PyObject *); 
        /* same as struct above */
PyAPI_FUNC(int) PyCode_Addr2Line(PyCodeObject *, int);

/* for internal use only */
typedef struct _addr_pair {
        int ap_lower;
        int ap_upper;
} PyAddrPair;

/* Check whether lasti (an instruction offset) falls outside bounds
   and whether it is a line number that should be traced.  Returns
   a line number if it should be traced or -1 if the line should not.

   If lasti is not within bounds, updates bounds.
*/

PyAPI_FUNC(int) PyCode_CheckLineNumber(PyCodeObject* co,
                                       int lasti, PyAddrPair *bounds);

PyAPI_FUNC(PyObject*) PyCode_Optimize(PyObject *code, PyObject* consts,
                                      PyObject *names, PyObject *lineno_obj);

#ifdef __cplusplus
}
#endif
#endif /* !Py_CODE_H */
