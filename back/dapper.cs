using System.Collections.Generic;
using Godot;
using Microsoft.Data.Sqlite;
using Dapper;
using AccountModel;
using System.Data.Common;

public static class Database
{
    private static string DbPath =>
        ProjectSettings.GlobalizePath("user://data.db");

    private static SqliteConnection Open()
    {
        var c = new SqliteConnection($"Data Source={DbPath}");
        c.Open();
        return c;
    }

    public static void Init()
    {
        using var c = Open();
        c.Execute(
            """
            CREATE TABLE IF NOT EXISTS accounts (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                level INTEGER NOT NULL DEFAULT 1,
                streack INTEGER NOT NULL DEFAULT 0
            );

            CREATE INDEX IF NOT EXISTS idx_accounts_username
                ON accounts(username);
            """
        );
    }

    public static long CreateAccount(Account a)
    {
        using var c = Open();
        var id = c.ExecuteScalar<long>(
            """
            INSERT INTO accounts (username, level, streack)
            VALUES (@Username, @Level, @Streack)
            RETURNING id;
            """,
            a);
        a.Id = (int)id;
        return id;
    }

        public static Account GetById(int id)
    {
        using var c = Open();
        return c.QuerySingleOrDefault<Account>(
            """
            SELECT id, username, level, streack
            FROM accounts
            WHERE id = @id;
            """,
            new {id});
    }

    public static Account GetByUsername(string username)
    {
        using var c = Open();
        return c.QuerySingleOrDefault<Account>(
            """
            SELECT id, username, level, streack
            FROM accounts
            WHERE username = @username;
            """,
            new {username});
    }

    public static bool UpdateUsername(int id, string newName)
    {
        using var c = Open();
        try
        {
            var rows = c.Execute(
                """
                UPDATE accounts
                SET username = @newName
                WHERE id = @id;
                """,
                new { newName, id });
            return rows > 0;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return false;
        }
    }

    public static bool ChangeName(Account account, string newName, out string changeError)
    {
        if (!account.CanChangeName(newName, out changeError))
            return false;

        if (!UpdateUsername(account.Id, newName))
        {
            changeError = "! Имя занято";
            return false;
        }

        account.ChangeName(newName);
        return true;
    }
    public static bool UpdateLevel(int id, int newLvl)
    {
        using var c = Open();
        try
        {
            var rows = c.Execute(
                """
                UPDATE accounts
                SET Level = @newLvl
                WHERE id = @id;
                """,
                new { newLvl, id });
            return rows > 0;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return false;
        }
    }
    public static bool ChangeLvl(Account account, int newLvl)
    {
        if (!UpdateLevel(account.Id, newLvl))
        {
            return(account.LvlSet(newLvl));
        }
        return true;
    }
}
