namespace LojaGerenciamento.Application.Models
{
    public class Response<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public T? Data { get; set; }
        public List<string> Erros { get; set; } = new List<string>();

        public static Response<T> Ok(T data, string mensagem = "Operação realizada com sucesso")
        {
            return new Response<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Data = data,
            };
        }

        public static Response<T> Erro(string mensagem, List<string>? erros = null)
        {
            return new Response<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                Erros = erros ?? new List<string>()
            };
        }
        public static Response<T> Erro(string mensagem, string erro)
        {
            return new Response<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                Erros = new List<string> { erro }
            };
        }

    }
}
