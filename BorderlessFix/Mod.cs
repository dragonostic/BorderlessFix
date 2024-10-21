using GDWeave;
using BorderlessFix.Mods;

namespace BorderlessFix;

public class Mod : IMod {
    public Config Config;

    public Mod(IModInterface modInterface) {
        this.Config = modInterface.ReadConfig<Config>();
        modInterface.Logger.Information("BorderlessFix is active!");
        modInterface.RegisterScriptMod(new OptionsTweaks.OptionsMenuModifier());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
