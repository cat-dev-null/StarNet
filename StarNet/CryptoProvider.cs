using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;

namespace StarNet
{
    public class CryptoProvider
    {
        private AsymmetricCipherKeyPair KeyPair { get; set; }

        public AsymmetricKeyParameter PublicKey
        {
            get
            {
                return KeyPair.Public;
            }
        }

        public CryptoProvider(AsymmetricCipherKeyPair keyPair)
        {
            KeyPair = keyPair;
        }

        public byte[] SignMessage(byte[] message)
        {
            var signer = new RsaDigestSigner(new Sha512Digest());
            signer.Init(true, KeyPair.Private);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.GenerateSignature();
        }

        public bool VerifySignature(byte[] message, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            var signer = new RsaDigestSigner(new Sha512Digest());
            signer.Init(false, publicKey);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.VerifySignature(signature);
        }
    }
}