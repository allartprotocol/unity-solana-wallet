
namespace AllArt.Solana.Nft { 
    public interface iNftFile<T> {
        public string name { get; set; }
        public string extension { get; set; }
        public string externalUrl { get; set; }
        public T file { get; set; }
    }
}