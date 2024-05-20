using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
namespace Securing_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SuperHeroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            var superHero = new List<SuperHero> { };

            using var connection = new SqlConnection(_configuration.GetConnectionString("PATH"));
            IEnumerable<SuperHero> heroes = await SelectSuperHeroes(connection);

            return Ok(heroes);

         
        }
        [HttpGet("Get Email by Name")]
        public async Task<ActionResult<List<SuperHero>>> GetbyName(string name)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("PATH"));
            var heroes = await connection.QueryAsync<SuperHero>($"SELECT [ID]\r\n      ,[FirstName]\r\n      ,[LastName]\r\n      ,[Email]\r\n  FROM [EmailsDataDB].[dbo].[emailAddress]\r\n  where FirstName=@name", new { name });

            return Ok(heroes);
        }
        [HttpDelete("ByName")]
        public async Task<ActionResult<SuperHero>> DeleteRcord(string name)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("PATH"));
            await connection.ExecuteAsync("delete emailAddress where FirstName=@name\r\n", new {name}); //iF YOU ARE ADDING UPDATING,DELETING DATA THEN YOU NEED TO USE EXECEUT Statement
            return Ok(await SelectSuperHeroes(connection));
        }

        static async Task<IEnumerable<SuperHero>> SelectSuperHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("SELECT *\r\n  FROM emailAddress");
        }

        [HttpGet("By Store Proceedure")]
        public async Task<ActionResult<List<SuperHero>>> GetAll() {
            using var conection = new SqlConnection(_configuration.GetConnectionString("PATH"));
            var proceedure = "user.GetEmails";
          //  var param = new DynamicParameters(id);
            var heroes =await conection.QueryAsync<SuperHero>(proceedure, commandType: CommandType.StoredProcedure);
            return Ok(heroes);
        }

    }
}