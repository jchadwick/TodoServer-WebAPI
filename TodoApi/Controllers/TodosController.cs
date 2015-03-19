using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [RoutePrefix("todos")]
    public class TodosController : ApiController
    {
        private TodoApiContext db = new TodoApiContext();

        protected virtual string IpAddress
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }
        protected IQueryable<Todo> Todos
        {
            get
            {
                return db.Todos.Where(x => x.IpAddress == IpAddress);
            }
        }

        // GET: api/Todos
        [Route("")]
        public IQueryable<Todo> GetTodos()
        {
            return Todos;
        }

        // GET: api/Todos/5
        [Route("{id}", Name = "GetTodo")]
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> GetTodo(long id)
        {
            Todo todo = await Todos.FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        // PUT: api/Todos/5
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTodo(long id, Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await TodoExistsAsync(id))
            {
                return NotFound();
            }

            if (id != todo.Id)
            {
                return BadRequest();
            }

            db.Entry(todo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Todos
        [Route("")]
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> PostTodo(Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("GetTodo", new { id = todo.Id }, todo);
        }

        // DELETE: api/Todos/5
        [Route("{id}")]
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> DeleteTodo(long id)
        {
            Todo todo = await Todos.FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            return Ok(todo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Task<bool> TodoExistsAsync(long id)
        {
            return Task.FromResult(TodoExists(id));
        }

        private bool TodoExists(long id)
        {
            return Todos.Any(e => e.Id == id);
        }
    }
}