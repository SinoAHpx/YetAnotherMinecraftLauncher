namespace YetAnotherMinecraftLauncher.Models.Authentication;

public class EncryptionEntry
{
    public byte[] IV { get; set; }

    public byte[] Value { get; set; }
}