# Módulo Auth

Esqueleto preparado para a change `autenticacao-jwt-httponly`.

Implementação prevista:

- `Domain`: entidade `Usuario` e regras de credencial
- `Application`: casos de uso de login/logout/sessão
- `Infrastructure`: persistência e serviço JWT
- `Presentation`: endpoints `/api/auth/*`

Testes unitários devem ficar em `apps/api/__teste__/Modules/Auth/Application`.
