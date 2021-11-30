
namespace AllArt.Solana.Nft { 
    public interface iNftFile<T> {
        string name { get; set; }
        string extension { get; set; }
        string externalUrl { get; set; }
        T file { get; set; }
    }
}