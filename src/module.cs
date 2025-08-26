using System.IO;
using System.Runtime.InteropServices;

using Fahrenheit.Core;

namespace Fahrenheit.Modules.TrueRNG;

/* [fkelava 9/9/24 22:26]
 * Every function you intend to hook or invoke must have an associated _delegate_ declared, i.e. a function pointer typedef.
 *
 * They must be annotated with [UnmanagedFunctionPointer(CallingConvention.YOUR_CALLING_CONVENTION_HERE)],
 * and have a matching signature to the function you are trying to hook or invoke.
 *
 * The signature is a union of the parameter list, calling convention, and return type. One delegate fits _all_ functions with the same signature.
 *
 * The name of the delegate is left free for you to declare. Here I chose to use the function's original name.
 *
 * Given the function signature
 * > uint __cdecl brnd(int param_1)
 *
 * the correct declaration is:
 * > [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
 * > public delegate uint brnd(int param_1);
 */

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate uint brnd(int param_1);

[FhLoad(FhGameType.FFX)]
public class TrueRNGModule : FhModule {
    /* [fkelava 9/9/24 22:26]
     * Every function you intend to hook or invoke must have an associated _method handle_ declared.
     *
     * Declare them as follows:
     * > private readonly FhMethodHandle<DELEGATE_TYPE_HERE> _handle;
     *
     * Initialize them in the constructor as follows:
     * > _handle = new FhMethodHandle<DELEGATE_TYPE_NAME>(this, EXE_OR_DLL_NAME, new DELEGATE_TYPE_NAME(HOOK_FUNCTION_NAME), [opt] OFFSET_IN_EXE_OR_DLL, [opt] NAME_OF_FUNCTION_IN_EXE_OR_DLL);
     *
     * where one of the last two parameters is required, whichever one; where HOOK_FUNCTION_NAME is the name of a function whose signature matches the delegate DELEGATE_TYPE_NAME.
     */
    private readonly FhMethodHandle<brnd> _brnd_handle;

    public TrueRNGModule() {
        _brnd_handle = new FhMethodHandle<brnd>(this, "FFX.exe", 0x398900, h_brnd);
    }

    public uint h_brnd(int param_1) {
        /* [fkelava 9/9/24 22:26]
         * Call the original function of a method handle as follows:
         * > _handle.orig_fptr.Invoke(...);
         *
         * DO NOT store the value of _handle.orig_fptr across _handle.hook() calls, as _handle.hook() mutates it!
         * DO NOT issue calls through orig_fptr concurrently with a _handle.hook() call!
         */
        return _brnd_handle.orig_fptr.Invoke(0);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        /* [fkelava 9/9/24 22:26]
         * Hook the target function of a method handle as follows:
         * > bool hook_successful = _handle.hook();
         *
         * DO NOT store the value of _handle.orig_fptr across _handle.hook() calls, as _handle.hook() mutates it!
         * DO NOT issue calls through orig_fptr concurrently with a _handle.hook() call!
         */

        return _brnd_handle.hook();
    }
}
