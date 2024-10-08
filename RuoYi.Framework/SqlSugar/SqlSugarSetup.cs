﻿using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.Extensions;

namespace RuoYi.Framework
{
    public static class SqlSugarSetup
    {
        public static void AddSqlSugarScope(this IServiceCollection services)
        {
            var connectionConfigs = App.GetConfig<ConnectionConfig[]>("ConnectionConfigs");

            services.AddSqlSugarScope(connectionConfigs, db =>
            {
                foreach (var conn in connectionConfigs)
                {
                    string configId = Convert.ToString(conn.ConfigId);

                    // SQL执行前
                    db.GetConnectionScope(configId).Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //Console.WriteLine(sql);//输出sql
                        App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
                    };

                    //SQL执行完
                    db.GetConnectionScope(configId).Aop.OnLogExecuted = (sql, pars) => 
                    {
                        //Console.WriteLine("time:" + db.Ado.SqlExecutionTime.ToString());//输出SQL执行时间
                    };

                    //SQL报错
                    db.GetConnectionScope(configId).Aop.OnError = (exp) =>
                    {
                        //exp.sql 这样可以拿到错误SQL      
                    };

                    //可以修改SQL和参数的值
                    //db.Aop.OnExecutingChangeSql = (sql, pars) => 
                    //{
                    //    return new KeyValuePair<string, SugarParameter[]>(sql, pars);s
                    //};

                    // Sql执行完后会进该事件，该事件可以拿到更改前记录和更改后记录，执行时间等参数
                    db.GetConnectionScope(configId).Aop.OnDiffLogEvent = it =>
                    {
                        var beforeData = it.BeforeData; // 操作前记录  包含： 字段描述 列名 值 表名 表描述
                        var afterData = it.AfterData;   // 操作后记录   包含： 字段描述 列名 值  表名 表描述
                        var sql = it.Sql;
                        var parameter = it.Parameters;
                        var data = it.BusinessData;         // 这边会显示你传进来的对象
                        var time = it.Time;
                        var diffType = it.DiffType;         //enum insert 、update and delete  
                    };
                }
            });
        }

    }
}
