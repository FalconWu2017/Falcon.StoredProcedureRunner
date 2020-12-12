using Microsoft.Extensions.DependencyInjection;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 扩展服务集合注册接口
    /// </summary>
    public static class IServiceCollectionExtend
    {
        /// <summary>
        /// 注册IRunner接口对象，通过该接口可事先 对数据库存储过程模型化调用。
        /// </summary>
        /// <param name="services">注册服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection UseFalconSPRunner(this IServiceCollection services) {
            return services.AddSingleton<IRunner,Runner>();
        }

    }
}
