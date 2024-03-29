using ToDoList.Application.DTOs;
using ToDoList.Application.Interfaces;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Interfaces;

namespace ToDoList.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISenhaService _senhaService;

    public AuthService(IUsuarioRepository usuarioRepository, ISenhaService senhaService)
    {
        _usuarioRepository = usuarioRepository;
        _senhaService = senhaService;
    }

    public async Task<ServiceResponse<string>> Login(LoginDto loginDto)
    {
        var serviceResponse = new ServiceResponse<string>();

        try
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorEmail(loginDto.Email);

            if (usuario is null)
            {
                serviceResponse.Dados = null;
                serviceResponse.Mensagem = "Credenciais inválida";
                serviceResponse.Sucesso = false;
                return serviceResponse;
            }

            if (!_senhaService.VerificaSenhaHashValida(loginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
            {
                serviceResponse.Dados = null;
                serviceResponse.Mensagem = "Credenciais inválida";
                serviceResponse.Sucesso = false;
                return serviceResponse;
            }

            var token = _senhaService.GerarToken(usuario);

            serviceResponse.Dados = token;
            serviceResponse.Mensagem = "Usuário logado com sucesso";
        }
        catch (Exception ex)
        {
            serviceResponse.Dados = null;
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Sucesso = false;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<UsuarioDto>> Registrar(UsuarioDto usuarioDto)
    {
        var serviceResponse = new ServiceResponse<UsuarioDto>();

        try
        {
            if (UsuarioJaExiste(usuarioDto))
            {
                serviceResponse.Mensagem = "Email já cadastrado";
                serviceResponse.Sucesso = false;
                return serviceResponse;
            }

            _senhaService.CriarSenhaHash(usuarioDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

            var usuario = new Usuario(usuarioDto.Nome, usuarioDto.Email, senhaHash, senhaSalt);

            await _usuarioRepository.Registrar(usuario);

            serviceResponse.Mensagem = "Usuário cadastrado com sucesso!";
        }
        catch (Exception ex)
        {
            serviceResponse.Dados = null;
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Sucesso = false;
        }

        return serviceResponse;

    }

    private bool UsuarioJaExiste(UsuarioDto usuarioDto)
    {
        var usuarioBanco = _usuarioRepository.ObterUsuarioPorEmail(usuarioDto.Email);

        if (usuarioBanco is not null)
            return true;

        return false;
    }

}
