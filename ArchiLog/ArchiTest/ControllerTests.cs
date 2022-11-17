using ArchiLibrary.controllers;
using ArchiLibrary.Data;
using ArchiLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArchiTest
{
    public class Modal : BaseModel { }
    public class Context : BaseDbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Modal> Modals { get; set; }
    }
    public class Controller : BaseController<BaseDbContext, Modal>
    {
        public Controller(BaseDbContext context) : base(context)
        {
        }
    }

    public class ControllerTests
    {
        private readonly Context _context;
        private readonly Controller _controller;

        public ControllerTests()
        {
            _context = ContextGenerator.Generate();
            _controller = new Controller(_context);
        }

        [Fact]
        public async void GetById()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var id = _context.Modals.Add(new Modal()).Entity.ID;
            _context.SaveChanges();

            var result = await _controller.GetById(id);
            Assert.NotNull(result);
        }

        [Fact]
        public async void PostItem()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            await _controller.PostItem(new Modal());

            Assert.Single(_context.Modals);
        }
    }

    public static class ContextGenerator
    {
        public static Context Generate()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new Context(options);
        }
    }
}