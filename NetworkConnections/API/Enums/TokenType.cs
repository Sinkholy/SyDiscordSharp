namespace Http.Enums
{
    enum TokenType : byte
    {
        Bot,
        Bearer, //Я так понимаю Bearer - токен связанный с OAuth2 API
        WebHook //С веб-хуками я вообще так и не смог разобраться
    }
}
