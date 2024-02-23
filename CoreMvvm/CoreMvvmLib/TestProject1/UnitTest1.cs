using TestProject1.Interface;
using TestProject1.Model;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var collection = ServiceCollection.Create();

            Assert.NotNull(collection);
        }
        [Fact]
        public void ContainerCreation()
        {

            var collection = ServiceCollection.Create();
            var container = collection.CreateContainer();

            Assert.NotNull(collection);
            Assert.NotNull(container);
        }
        [Fact]
        public void SingletonObjectTest1()
        {

            var collection = ServiceCollection.Create();
            collection.AddSingleton<AModel>();
            collection.AddSingleton<IBModel, BModel>();

            var container1 = collection.CreateContainer();

            var aModel1 = container1.GetService<AModel>();
            var aModel2 = container1.GetService<AModel>();
            var bModel1 = container1.GetService<IBModel>();
            var bModel2 = container1.GetService<IBModel>();
           

            Assert.Equal(aModel1, aModel2);
            Assert.Equal(bModel1, bModel2);            
        }
        [Fact]
        public void OnCreateObjectTest1()
        {

            var collection = ServiceCollection.Create();
            collection.AddTransient<AModel>();
            collection.AddTransient<IBModel, BModel>();

            var container1 = collection.CreateContainer();

            var aModel1 = container1.GetService<AModel>();
            var aModel2 = container1.GetService<AModel>();
            var bModel1 = container1.GetService<IBModel>();
            var bModel2 = container1.GetService<IBModel>();

            Assert.NotEqual(aModel1, aModel2);
            Assert.NotEqual(bModel1, bModel2);
        }
    }
}