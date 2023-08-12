using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System.Security.Cryptography;
using System;

public class RSAKeyGen : MonoBehaviour
{
   void KeyGen(BigInteger PublicExponent, string P, string Q)
   {
      int keySizeInBits = 2048; // Specify the desired key size in bits
      BigInteger publicExponent = PublicExponent; // Specify the desired public exponent
      BigInteger p = BigInteger.Parse(P); // Specify the prime factor p
      BigInteger q = BigInteger.Parse(Q); // Specify the prime factor q
      BigInteger n = p * q; // modulus

      using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySizeInBits))
      {

         rsa.ImportParameters(GetFullPrivateParameters(p, q, publicExponent, n));

         // Export public key
         string publicKey = rsa.ToXmlString(false);
         Debug.Log("Public Key:\n" + publicKey);

         // Export private key
         string privateKey = rsa.ToXmlString(true);
         Debug.Log("Private Key:\n" + privateKey);
      }
   }

   static RSAParameters GetFullPrivateParameters(BigInteger p, BigInteger q, BigInteger e, BigInteger modulus)
   {
      var n = p * q;
      var phiOfN = n - p - q + 1; // OR: (p - 1) * (q - 1);

      var d = ModInverse(e, phiOfN);
      //Assert.Equal(1, (d * e) % phiOfN);

      var dp = d % (p - 1);
      var dq = d % (q - 1);

      var qInv = ModInverse(q, p);
      //Assert.Equal(1, (qInv * q) % p);

      return new RSAParameters
      {
         Modulus = CopyAndReverse(n.ToByteArray()),
         Exponent = CopyAndReverse(e.ToByteArray()),
         D = CopyAndReverse(d.ToByteArray()),
         P = CopyAndReverse(p.ToByteArray()),
         Q = CopyAndReverse(q.ToByteArray()),
         DP = CopyAndReverse(dp.ToByteArray()),
         DQ = CopyAndReverse(dq.ToByteArray()),
         InverseQ = CopyAndReverse(qInv.ToByteArray()),
      };
   }

   static BigInteger ModInverse(BigInteger a, BigInteger n)
   {
      BigInteger t = 0, nt = 1, r = n, nr = a;

      if (n < 0)
      {
         n = -n;
      }

      if (a < 0)
      {
         a = n - (-a % n);
      }

      while (nr != 0)
      {
         var quot = r / nr;

         var tmp = nt; nt = t - quot * nt; t = tmp;
         tmp = nr; nr = r - quot * nr; r = tmp;
      }

      if (r > 1) throw new ArgumentException(nameof(a) + " is not convertible.");
      if (t < 0) t = t + n;
      return t;
   }

   static byte[] CopyAndReverse(byte[] data)
   {
      byte[] reversed = new byte[data.Length];
      Array.Copy(data, 0, reversed, 0, data.Length);
      Array.Reverse(reversed);
      return reversed;
   }
}
