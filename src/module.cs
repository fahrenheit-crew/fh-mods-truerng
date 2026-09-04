// SPDX-License-Identifier: MIT

using System.IO;

namespace Fahrenheit.Mods.TrueRNG;

[FhLoad(FhGameId.FFX)]
public class TrueRNGModule : FhModule {
    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        // Hook the target function through FhCall:
        //   {|FFX|FFX2}.FhCall.function_name.hook(this, hook_method);
        return FhCall.brnd.hook(this, h_brnd);
    }

    public int h_brnd(int slot) {
        // The game's RNG has multiple "slots", each capable of producing a distinct random number sequence,
        // which is what allows speedrunners to track most of the randomness.

        // To prevent this, we ignore the slot parameter and always return random numbers from slot 0.
        // This causes the untrackable event RNG to affect all other RNG results, making them all untrackable.

        // Fall through to the original function (including other hooks) using `.chain_from(hook_method)`:
        //   {|FFX|FFX2}.FhCall.function_name.chain_from(hook_method).fnptr!(arguments);
        return FhCall.brnd.chain_from(h_brnd).fnptr!(0);
    }
}
