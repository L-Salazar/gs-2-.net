namespace RemoteReady.Models
{
    public class PageData<T>
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public IEnumerable<T> Itens { get; set; }

        public PageData(int paginaAtual, int totalPaginas, int totalRegistros, IEnumerable<T> itens)
        {
            PaginaAtual = paginaAtual;
            TotalPaginas = totalPaginas;
            TotalRegistros = totalRegistros;
            Itens = itens;
        }
    }
}
