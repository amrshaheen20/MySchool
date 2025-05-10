using System.Linq.Expressions;

namespace MySchool.API.Common
{
    public class Command<T> where T : class
    {
        public required Expression<Func<IQueryable<T>, IQueryable<T>>> Value { get; set; }
    }

    public class CommandsInjector<T> where T : class
    {
        public CommandsInjector() { }

        protected List<object> _commands = new List<object>();

        public CommandsInjector<T> AddCommand(Expression<Func<IQueryable<T>, IQueryable<T>>> Command, int? Index = default)
        {
            _commands.Add(new Command<T>
            {
                Value = Command
            });
            return this;
        }

        public CommandsInjector<T> AddCommands(params Expression<Func<IQueryable<T>, IQueryable<T>>>[] Commands)
        {
            foreach (var command in Commands)
            {
                AddCommand(command);
            }
            return this;
        }

        public IQueryable<T> ApplyCommand(IQueryable<T> query)
        {
            foreach (var command in _commands.OfType<Command<T>>())
            {
                var compiledFunc = command.Value.Compile();
                query = compiledFunc(query);
            }

            return query;
        }


        //IQueryable functions
        public CommandsInjector<T> Where(Expression<Func<T, bool>> predicate)
        {
            var command = new Command<T>
            {
                Value = q => q.Where(predicate)
            };
            _commands.Add(command);
            return this;
        }


    }

}
