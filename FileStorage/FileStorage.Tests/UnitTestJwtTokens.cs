using System.Diagnostics;
using System.Net.Http.Headers;

namespace FileStorage.Tests
{
    public class UnitTestJwtTokens
    {
        HttpRequestMessage _request = new HttpRequestMessage(HttpMethod.Options, "https://localhost:7089/v1/healthz");
        private HttpClient _client = new();
        private string[] _jwtTokens = {
            // Normal
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiZmM5NWU1M2UtOWJjNS00Y2NmLThiNDAtYTc2YWUxYWJhOWM2IiwiaWF0IjoxNjk3MzUxMzQ1LCJuYmYiOjE2OTczNTEzNDUsImV4cCI6MTY5NzM1MTQwNX0.kedBfrIpmhCJWlOlHmrF7icJkFCe3MlgTiMQN3ZRy2I",
            // Normal without Bearer
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiNjYyYTJlNzMtMGNmMi00NTZmLWE4OWQtMjg5NmY5ZGJjMjhmIiwiaWF0IjoxNjk3MzQ3MjY0LCJuYmYiOjE2OTczNDcyNjQsImV4cCI6MTY5NzM0NzMyNH0.OZ5Zj02dgrYzRLzun7oZ2OGRr7rMWWKcvgchWLDwwYg",
            // Without user id
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiYWMzNjQ2YTUtNjEyYi00NDkzLWJmYzEtZjE3NTFmNWRhZGVmIiwiaWF0IjoxNjk3MzUxNTI0LCJuYmYiOjE2OTczNTE1MjQsImV4cCI6MTY5NzM1MTU4NH0.8fe_pWRk4xK5PlELMqeyVB42GYdFk0er82yJTYXkjas",
            // Wrong secret key
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiNDdlMmM5MWMtZmVmNS00MTBhLWJkMWQtNjJjZmE1ZjczM2VjIiwiaWF0IjoxNjk3MzQ3NDkyLCJuYmYiOjE2OTczNDc0OTIsImV4cCI6MTY5NzM0NzU1Mn0.0FasfJ19Of5AB6mGEtKRXQ65ybaIYx024D4JCA628ks",
            // Wrong secret without user id
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiNjljZmJmM2EtMmQ1ZS00M2Q3LWIzZWEtZTVkZmYyYTIyOTBjIiwiaWF0IjoxNjk3MzQ3NjE1LCJuYmYiOjE2OTczNDc2MTUsImV4cCI6MTY5NzM0NzY3NX0.j0kpTp5Hm6P9gFlosJ98WAbjoL87eT9ZgEf1HDrRaJ8",
            // HmacSha256Signature Normal
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiNjM0ZDUzNjctNjYxOS00ZWU4LThhMjktZDAwZmI5M2ExMmJmIiwiaWF0IjoxNjk3MzQ4MTA4LCJuYmYiOjE2OTczNDgxMDgsImV4cCI6MTY5NzM0ODE2OH0.Pym5FXS2SI9KDPKvgjUeanHFgJSgMD_dHmJlsDInfyg",
            // HmacSha256Signature Normal without Bearer
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiNjM0ZDUzNjctNjYxOS00ZWU4LThhMjktZDAwZmI5M2ExMmJmIiwiaWF0IjoxNjk3MzQ4MTA4LCJuYmYiOjE2OTczNDgxMDgsImV4cCI6MTY5NzM0ODE2OH0.Pym5FXS2SI9KDPKvgjUeanHFgJSgMD_dHmJlsDInfyg",
            // HmacSha256Signature Without user id
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiZGM5MjE1NTItMjc0Ny00MGY0LTk3ZjItOThlZDBiOGZkODAzIiwiaWF0IjoxNjk3MzQ4NjAxLCJuYmYiOjE2OTczNDg2MDEsImV4cCI6MTY5NzM0ODY2MX0.2rMT6uW-8iXwLQh4rYvNl2HzG3Lx8O3kVgdpE_-av6Q",
            // HmacSha256Signature Wrong secret key
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiMWVhMjA2N2QtMTM5OS00YzBlLTliZjctN2M0MDc5MGYwZTc3IiwiaWF0IjoxNjk3MzQ4NjcxLCJuYmYiOjE2OTczNDg2NzEsImV4cCI6MTY5NzM0ODczMX0.7bO55xv2NiYyyRKwGzHschNU9k-eMw_X5Vn0qxmkJK0",
            // HmacSha256Signature Wrong secret without user id
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQ2xpZW50Iiwic3ViIjoidGVzdEBnbWFpbC5jb20iLCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwianRpIjoiMmY0MDYyMmItMGExMi00ZWJjLWFiOTktOTE0Y2YxNWIzYmI4IiwiaWF0IjoxNjk3MzQ4NjUxLCJuYmYiOjE2OTczNDg2NTEsImV4cCI6MTY5NzM0ODcxMX0.IMU9r40mWGzuOK7ZGr3An1Htzx46fL2onQrTSxp2U7o"
        };

        [Fact]
        public async void TestHmacSha256NormalJwt_Expect200()
        {
            // arrange
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[0]);
            // act
            var response = await _client.SendAsync(_request);
            // assert
            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256JwtWithoutBearer_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[1]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256JwtWithoutUserId_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[2]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256JwtWrongSecret_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[3]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256JwtWithoutUserIdAndWrongSecret_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[4]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }



        [Fact]
        public async void TestHmacSha256Signature_NormalJwt_Expect200()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[5]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256Signature_JwtWithoutBearer_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[6]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256Signature_JwtWithoutUserId_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[7]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256Signature_JwtWrongSecret_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[8]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async void TestHmacSha256Signature_JwtWithoutUserIdAndWrongSecret_Expect401()
        {
            _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", _jwtTokens[9]);
            var response = await _client.SendAsync(_request);
            Assert.Equal(401, (int)response.StatusCode);
        }
    }
}