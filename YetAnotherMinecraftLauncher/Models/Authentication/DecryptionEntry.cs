namespace YetAnotherMinecraftLauncher.Models.Authentication;

public class DecryptionEntry
{
    public byte[] IV { get; set; }

    public string Value { get; set; }
}