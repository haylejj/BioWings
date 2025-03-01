using System.Text;

namespace BioWings.Application.Helper;
public static class RandomPasswordHelper
{
    public static string GenerateRandomPassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.-*/!.";
        StringBuilder res = new();
        Random rnd = new();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }
}
