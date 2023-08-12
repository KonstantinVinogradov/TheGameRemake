using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

public class SaveManager : MonoBehaviour
{
   private static Save sv = new();
   private string path;
   public static SaveManager Instance;
   private RSA rsa = RSA.Create();

   public TextMeshProUGUI BestScore;

   private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
   {
      Formatting = Formatting.Indented,
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
   };

   private void Start()
   {
      string privateKeyXml = "<RSAKeyValue><Modulus>pCkoItDgVw+sPNmNhlhudaTForWoS8qmRzxDfX5i3wRczZ7lepGQPcQN64wacjUmmmOU8DdU4MnaPPBmyO5SWdUpvHaNpPeKwBP7NhcrWfYT1JUTID+in/WDPaI5+jUryTmNF05TnTkKCAjVhSPbQ5Oe96z+TZkwXKQroS/UfNu0ykv/Xwt1q7J0wNGbccjw8yzqvsWl/zb97ox0LfvDpXlcSakUt1HWnDotB2ERxPDp1kCsqMZGcFkpsuY2KNyjPnxAI9L36va1Irq3QjVxb7ILayOrlq1peXAg58/3iK26eoJ7FjTn1CbjDS9aLUf20bqMptD/CogR+QC9taZ8Cw==</Modulus><Exponent>R7UF1w==</Exponent><P>rdBiR8Bnlc+sCAbUrMJVWZqyFRIiHHqceHwIi/i4c9qFXlMre1VnU0thFJo64YmsgozA1bXf8c7Rw2zH5Opf/g1rG0A53ouYs+ELU8wnDXcCRgF1poyjQAqcnjkZaJaZbaaaWGKKOwjBgCwqmZr/0VHSV8R+aeKJIhLN4vA59i8=</P><Q>8chH5U+SYcWQQCcg6p1NIr2LQIhMqPnoHCp5Tpk+o0LdxX6aSmBnebR7J5LHM681HvWDKCK6KouhfLmV8a61w1kJjkX5dpJmEfq8hgs5gAdcI/f0v0P1PVhnWFa/IcfTXxwDTp/565BIKbm/1ZBpoeAY7SbqXwp+KyjJiX05/OU=</Q><DP>P1Qta7dpM3o9q2JlNxSlif10ieW5sK1iDNN/d+7LzAqMUaFES02ooA5gKA4W1FNuOrHFcn49mKILeG52lyDhXwPrLKTO3M3O0fgaNptGEta9bxvwzvfGg6Sphigw5yJuXxW2EBalA7Scmch9qtrEm5bdx2WfID2wb/HRkemYuTM=</DP><DQ>SRuRtm8TVoug7vLYRf4cY1oO732mk+4fr5AmmT4KjlXMrXM8Hzv5ctbu8yt/1kg/rvFnolq4jncUCLQSWW3hQhNDf1+tLGxc3VBbui48AhSDYz99ukYe0YwpYXvusT2TP28OF5Jo0p5sPnveu4oIf7Y9cm1KYJjnM3V/npT8FMM=</DQ><InverseQ>nqNrwn5aTzp8pkxa5ZukVUdQC2VPpnwrKyzJlv8COdhok6eL+cDxK2QbzPL45VWKlTztuD9qz58wyVYYhDm/8L6Ccw1Er6G+C8OPPhUe//IQ/xhj4FOcmWWucZkz1LixjpU1Y2edr9xnYXuE0I9erIbkeVRo0Aig3t24W+wNE/E=</InverseQ><D>EWXlQYLOE0m17E0iBQKBfiFkE7aKyQuNrIXfUy5CS0In6C2GpV7K/3S4ABad37/T83SEdIb5zIIzdaMp5vqRFoXMqi3hWdJzOSOWaWVK3qNTuR0K5YC8Dg1PcrakSbAucrwcGj8JJC7f7c4ILg1+X6c4fG9dFVXcOI7siey01sUzNFQLCob+qXVstCqsNRZZEB4g/4JC6Teylk2V8LKVcSV/KGfJWXZb6of+2UYwpTQweRthjwktAmUk+ELhcUAnXRgEI0myGn6fqZKF8Jeb3xPx/Ibt9UB/+AsHy0Azlr0KU42nmwNlbxn37QIAmnaDIRbw33NV0fFt7TAX7Jqw9w==</D></RSAKeyValue>";
      if (Instance)
      {
         Destroy(this);
      }
      else
      {
         Instance = this;
         DontDestroyOnLoad(Instance);
      }
#if UNITY_ANDROID && !UNITY_EDITOR
      path = Path.Combine(Application.persistentDataPath, Save.json);
#else
      path = Path.Combine(Application.dataPath, "Save.json");
#endif
      if (File.Exists(path))
      {
         sv = JsonConvert.DeserializeObject<Save>(Decrypt(privateKeyXml, File.ReadAllText(path)));
      }


      BestScore.text = "Best Score:\n" + sv.BestScore.ToString();
   }

   public void SaveScore(int score)
   {
      if (score > sv.BestScore)
      {
         sv.BestScore = score;
         BestScore.text = "Best Score:\n" + score.ToString();
      }
   }

#if UNITY_ANDROID && !UNITY_EDITOR
   private void ONApplicationPause(bool pause)
   {
      if (pause)
      {
         string publicKeyXml  = "<RSAKeyValue><Modulus>pCkoItDgVw+sPNmNhlhudaTForWoS8qmRzxDfX5i3wRczZ7lepGQPcQN64wacjUmmmOU8DdU4MnaPPBmyO5SWdUpvHaNpPeKwBP7NhcrWfYT1JUTID+in/WDPaI5+jUryTmNF05TnTkKCAjVhSPbQ5Oe96z+TZkwXKQroS/UfNu0ykv/Xwt1q7J0wNGbccjw8yzqvsWl/zb97ox0LfvDpXlcSakUt1HWnDotB2ERxPDp1kCsqMZGcFkpsuY2KNyjPnxAI9L36va1Irq3QjVxb7ILayOrlq1peXAg58/3iK26eoJ7FjTn1CbjDS9aLUf20bqMptD/CogR+QC9taZ8Cw==</Modulus><Exponent>R7UF1w==</Exponent></RSAKeyValue>";
         rsa.FromXmlString(publicKeyXml);
         string Data2Encrypt = JsonConvert.SerializeObject(sv, Settings);
         string encryptedString = Encrypt(publicKeyXml, Data2Encrypt);
         File.WriteAllText(path, encryptedString);
      }
   }
#endif

   private void OnApplicationQuit()
   {
      string publicKeyXml  = "<RSAKeyValue><Modulus>pCkoItDgVw+sPNmNhlhudaTForWoS8qmRzxDfX5i3wRczZ7lepGQPcQN64wacjUmmmOU8DdU4MnaPPBmyO5SWdUpvHaNpPeKwBP7NhcrWfYT1JUTID+in/WDPaI5+jUryTmNF05TnTkKCAjVhSPbQ5Oe96z+TZkwXKQroS/UfNu0ykv/Xwt1q7J0wNGbccjw8yzqvsWl/zb97ox0LfvDpXlcSakUt1HWnDotB2ERxPDp1kCsqMZGcFkpsuY2KNyjPnxAI9L36va1Irq3QjVxb7ILayOrlq1peXAg58/3iK26eoJ7FjTn1CbjDS9aLUf20bqMptD/CogR+QC9taZ8Cw==</Modulus><Exponent>R7UF1w==</Exponent></RSAKeyValue>";
      rsa.FromXmlString(publicKeyXml);
      string Data2Encrypt = JsonConvert.SerializeObject(sv, Settings);
      string encryptedString = Encrypt(publicKeyXml, Data2Encrypt);
      File.WriteAllText(path, encryptedString);
   }

   public static string Encrypt(string publicKeyXml, string plainText)
   {
      using (RSA rsa = RSA.Create())
      {
         rsa.FromXmlString(publicKeyXml);
         byte[] encodedText = Encoding.UTF8.GetBytes(plainText);
         byte[] encryptedBytes = rsa.Encrypt(encodedText, RSAEncryptionPadding.Pkcs1);
         return Convert.ToBase64String(encryptedBytes);
      }
   }

   public static string Decrypt(string privateKeyXml, string encryptedText)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.FromXmlString(privateKeyXml);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}


[Serializable]
public class Save
{
   public int BestScore { get; set; }
}