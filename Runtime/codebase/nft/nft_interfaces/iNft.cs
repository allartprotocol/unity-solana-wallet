
namespace AllArt.Solana.Nft { 
    public interface iNftStandard <T> {
        abstract T ParseData(string base64Data);
    }
}

