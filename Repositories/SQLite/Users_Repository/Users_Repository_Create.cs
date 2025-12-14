using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Security.JWT;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;
using System.Text;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Repository_Create : IUsers_Repository_Create
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;
        private readonly IUsers_Repository_Update IUsers_Repository_Update;
        private readonly IUsers_Repository_Read IUsers_Repository_Read;

        public Users_Repository_Create(
            Users_Database_Context Users_Database_Context,
            Constants constants,
            IAES aes,
            IJWT jwt,
            IPassword password,
            IUsers_Repository_Update iuser_repository_update,
            IUsers_Repository_Read iuser_repository_read
        )
        {
            _UsersDBC = Users_Database_Context;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Password = password;
            IUsers_Repository_Update = iuser_repository_update;
            IUsers_Repository_Read = iuser_repository_read;
        }

        private async Task<string> Generate_User_Public_ID()
        {
            string user_public_id;
            Random random = new();

            do
            {
                user_public_id = new string(Enumerable
                    .Repeat("0123456789", 5)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            } while (await _UsersDBC.User_IDsTbl.AnyAsync(x => x.Public_ID == user_public_id));

            return user_public_id;
        }
        private async Task<User_Secret_DTO> Generate_User_Secret_ID()
        {
            Random random = new();
            string user_secret_id;
            string user_secret_hash_id;
            string user_encrypted_secret;
            string character_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            do
            {
                user_secret_id = $@"
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}
                ";

                user_secret_hash_id = SHA256_Generator.ComputeHash(user_secret_id);

                user_encrypted_secret = AES.Process_Encryption(user_secret_id);

            } while (await _UsersDBC.User_IDsTbl.AnyAsync(x => x.Secret_Hash_ID == user_secret_hash_id));

            return new User_Secret_DTO
            {
                Encryption = user_encrypted_secret,
                Hash = user_secret_hash_id
            };
        }
        public async Task<string> Create_Reported_Record(Reported dto)
        {
            try
            {
                switch (dto.Report_type.ToUpper())
                {
                    case "ABUSE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Abuse = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_abuse_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_abuse_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Abuse = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_abuse_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_abuse_record_exists_in_database.Updated_on = TimeStamp();
                            reported_abuse_record_exists_in_database.Abuse = reported_abuse_record_exists_in_database.Abuse + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_abuse_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            abuse_record_created_on = TimeStamp(),
                        });

                    case "DISRUPTION":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Disruption = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_disruption_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_disruption_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Disruption = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_disruption_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_disruption_record_exists_in_database.Updated_on = TimeStamp();
                            reported_disruption_record_exists_in_database.Disruption = reported_disruption_record_exists_in_database.Disruption + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_disruption_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            disruption_record_created_on = TimeStamp(),
                        });

                    case "SELF_HARM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Self_harm = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_self_harm_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_self_harm_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Self_harm = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_self_harm_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_self_harm_record_exists_in_database.Updated_on = TimeStamp();
                            reported_self_harm_record_exists_in_database.Self_harm = reported_self_harm_record_exists_in_database.Self_harm + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_self_harm_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            self_harm_record_created_on = TimeStamp(),
                        });

                    case "SPAM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Spam = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_spam_harm_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_spam_harm_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Spam = 1
                            };

                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);

                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }

                        }
                        else
                        {

                            reported_spam_harm_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_spam_harm_record_exists_in_database.Updated_on = TimeStamp();
                            reported_spam_harm_record_exists_in_database.Self_harm = reported_spam_harm_record_exists_in_database.Self_harm + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_spam_harm_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            spam_record_created_on = TimeStamp(),
                        });

                    case "ILLEGAL":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Illegal = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_illegal_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_illegal_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Illegal = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_illegal_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_illegal_record_exists_in_database.Updated_on = TimeStamp();
                            reported_illegal_record_exists_in_database.Illegal = reported_illegal_record_exists_in_database.Illegal + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_illegal_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            illegal_record_created_on = TimeStamp(),
                        });

                    case "HARASS":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Harass = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_harass_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_harass_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Harass = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_harass_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_harass_record_exists_in_database.Updated_on = TimeStamp();
                            reported_harass_record_exists_in_database.Harass = reported_harass_record_exists_in_database.Harass + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_harass_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            harass_record_created_on = TimeStamp(),
                        });

                    case "MISINFORM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Misinform = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_misinform_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_misinform_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Misinform = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_misinform_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_misinform_record_exists_in_database.Updated_on = TimeStamp();
                            reported_misinform_record_exists_in_database.Misinform = reported_misinform_record_exists_in_database.Misinform + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_misinform_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            misinform_record_created_on = TimeStamp(),
                        });

                    case "NUDITY":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Nudity = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_nudity_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_nudity_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Nudity = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_nudity_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_nudity_record_exists_in_database.Updated_on = TimeStamp();
                            reported_nudity_record_exists_in_database.Nudity = reported_nudity_record_exists_in_database.Nudity + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_nudity_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            nudity_record_created_on = TimeStamp(),
                        });

                    case "FAKE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Fake = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_fake_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_fake_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Fake = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_fake_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_fake_record_exists_in_database.Updated_on = TimeStamp();
                            reported_fake_record_exists_in_database.Fake = reported_fake_record_exists_in_database.Fake + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_fake_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            fake_record_created_on = TimeStamp(),
                        });

                    case "HATE_SPEECH":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Hate = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_hate_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_hate_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Hate = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_hate_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_hate_record_exists_in_database.Updated_on = TimeStamp();
                            reported_hate_record_exists_in_database.Hate = reported_hate_record_exists_in_database.Hate + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_hate_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            hate_record_created_on = TimeStamp(),
                        });

                    case "VIOLENCE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Violence = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_violence_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_violence_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Violence = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_violence_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_violence_record_exists_in_database.Updated_on = TimeStamp();
                            reported_violence_record_exists_in_database.Violence = reported_violence_record_exists_in_database.Violence + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_violence_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            violence_record_created_on = TimeStamp(),
                        });

                    default:
                        return "Server Error: Report Record Selection Failed.";
                }
            }
            catch
            {
                return "Server Error: Report Record Creation Failed.";
            }
        }
        public async Task<string> Insert_Report_Email_Registration(Report_Email_Registration dto)
        {
            try
            {
                await _UsersDBC.Report_Email_RegistrationTbl.AddAsync(new Report_Email_RegistrationTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Reported Email Registration Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Pending_Email_Registration_History(Report_Failed_Pending_Email_Registration_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Pending_Email_Registration_HistoryTbl.AddAsync(new Report_Failed_Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.Email_Address,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_User_Agent_History(Report_Failed_User_Agent_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_User_Agent_HistoryTbl.AddAsync(new Report_Failed_User_Agent_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    End_User_ID = dto.End_User_ID,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Login_type = dto.Login_type,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report User Agent Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Selected_History(Report_Failed_Selected_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Selected_HistoryTbl.AddAsync(new Report_Failed_Selected_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Selected History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Logout_History(Report_Failed_Logout_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Logout_HistoryTbl.AddAsync(new Report_Failed_Logout_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_JWT_History_Record(Report_Failed_JWT_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_JWT_HistoryTbl.AddAsync(new Report_Failed_JWT_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_id = dto.JWT_id,
                    Client_id = dto.Client_id,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    End_User_ID = dto.End_User_ID,
                    Login_type = dto.Login_type,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Client_ID_History_Record(Report_Failed_Client_ID_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Client_ID_HistoryTbl.AddAsync(new Report_Failed_Client_ID_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Unregistered_Email_Login_History_Record(Report_Failed_Unregistered_Email_Login_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Unregistered_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Unregistered_Email_Login_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.Email_Address,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Unregistered Email Login History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Email_Login_History_Record(Report_Failed_Email_Login_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Email_Login_HistoryTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = dto.Reason,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb

                });

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Email Login History Failed.";
            }
        }
        public async Task<string> Insert_End_User_Logout_History_Record(Logout_Time_Stamp dto)
        {
            await _UsersDBC.Logout_Time_Stamp_HistoryTbl.AddAsync(new Logout_Time_Stamp_HistoryTbl
            {
                End_User_ID = dto.End_User_ID,
                Logout_on = TimeStamp(),
                Updated_by = dto.End_User_ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Client_time = dto.Client_Time_Parsed,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return Task.FromResult(JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                logout_on = TimeStamp()
            })).Result;
        }
        public async Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_History dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_Registration_HistoryTbl.AddAsync(new Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_time = dto.Client_time,
                    Code = dto.Code,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    email_address = dto.Email_Address,
                    language = dto.Language_Region.Split("-")[0],
                    region = dto.Language_Region.Split("-")[1],
                    updated_on = TimeStamp(),
                });
            }
            catch
            {
                return "Server Error: Email Address Registration Failed";
            }
        }
        public async Task<User_Data_DTO> Create_Account_By_Email(Complete_Email_Registration dto)
        {
            string user_public_id = await Generate_User_Public_ID();
            User_Secret_DTO user_secret = await Generate_User_Secret_ID();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                Public_ID = user_public_id,
                Secret_ID = user_secret.Encryption,
                Secret_Hash_ID = user_secret.Hash,
                Created_by = 0,
                Created_on = TimeStamp(),
                Updated_on = TimeStamp(),
                Updated_by = 0
            };

            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, true)
                .SetProperty(col => col.Deleted_on, TimeStamp())
                .SetProperty(col => col.Updated_on, TimeStamp())
                .SetProperty(col => col.Updated_by, ID_Record.ID)
                .SetProperty(col => col.Deleted_by, ID_Record.ID)
                .SetProperty(col => col.Client_time, dto.Client_time)
                .SetProperty(col => col.Server_Port, dto.Server_Port)
                .SetProperty(col => col.Server_IP, dto.Server_IP)
                .SetProperty(col => col.Client_Port, dto.Client_Port)
                .SetProperty(col => col.Client_IP, dto.Client_IP)
                .SetProperty(col => col.Client_IP, dto.Remote_IP)
                .SetProperty(col => col.Client_Port, dto.Remote_Port)
                .SetProperty(col => col.User_agent, dto.User_agent)
                .SetProperty(col => col.Window_width, dto.Window_width)
                .SetProperty(col => col.Window_height, dto.Window_height)
                .SetProperty(col => col.Screen_width, dto.Screen_width)
                .SetProperty(col => col.Screen_height, dto.Screen_height)
                .SetProperty(col => col.RTT, dto.RTT)
                .SetProperty(col => col.Orientation, dto.Orientation)
                .SetProperty(col => col.Data_saver, dto.Data_saver)
                .SetProperty(col => col.Color_depth, dto.Color_depth)
                .SetProperty(col => col.Pixel_depth, dto.Pixel_depth)
                .SetProperty(col => col.Connection_type, dto.Connection_type)
                .SetProperty(col => col.Down_link, dto.Down_link)
                .SetProperty(col => col.Device_ram_gb, dto.Device_ram_gb)
            );

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Updated_by = ID_Record.ID,
                Deleted = true,
                Deleted_by = ID_Record.ID,
                Deleted_on = TimeStamp(),
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Code = dto.Code,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Client_time = dto.Client_time,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                Name = $@"{dto.Name}",
                End_User_ID = ID_Record.ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                End_User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                End_User_ID = ID_Record.ID,
                Password = Password.Create_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                End_User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Alignment(new Selected_App_Alignment
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Nav_Lock(new Selected_Navbar_Lock
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Text_Alignment(new Selected_App_Text_Alignment
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Theme(new Selected_Theme
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme
            });

            await IUsers_Repository_Update.Update_End_User_Account_Roles(new Account_Role
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Groups(new Account_Group
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Type(new Account_Types
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Grid_Type(new Selected_App_Grid_Type
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Status(new Selected_Status
            {
                End_User_ID = ID_Record.ID,
                Status = 2,
            });

            string token = JWT.Create_Email_Account_Token(new JWT_DTO
            {
                End_User_ID = ID_Record.ID,
                User_groups = "0",
                User_roles = "User",
                Account_type = 1,
                Email_address = dto.Email_Address
            }).Result;

            await IUsers_Repository_Update.Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP = dto.Server_IP,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                login_type = "email",
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                id = ID_Record.ID,
                name = $@"{dto.Name}#{user_public_id}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0"
            };
        }
        public async Task<User_Data_DTO> Create_Account_By_Twitch(Complete_Twitch_Registeration dto)
        {
            string user_public_id = await Generate_User_Public_ID();
            User_Secret_DTO user_secret = await Generate_User_Secret_ID();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                Public_ID = user_public_id,
                Secret_ID = user_secret.Encryption,
                Secret_Hash_ID = user_secret.Hash,
                Created_by = 0,
                Created_on = TimeStamp(),
                Updated_on = TimeStamp(),
                Updated_by = 0
            };

            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Twitch_IDsTbl.AddAsync(new Twitch_IDsTbl
            {
                End_User_ID = ID_Record.ID,
                Twitch_ID = dto.Twitch_ID,
                User_Name = dto.Twitch_User_Name,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 9
            });

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                Name = $@"{dto.Twitch_Name}",
                End_User_ID = ID_Record.ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Twitch_Email_AddressTbl.AddAsync(new Twitch_Email_AddressTbl
            {
                End_User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                End_User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Alignment(new Selected_App_Alignment
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Nav_Lock(new Selected_Navbar_Lock
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Text_Alignment(new Selected_App_Text_Alignment
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Theme(new Selected_Theme
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme
            });

            await IUsers_Repository_Update.Update_End_User_Account_Roles(new Account_Role
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Groups(new Account_Group
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Type(new Account_Types
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Grid_Type(new Selected_App_Grid_Type
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Status(new Selected_Status
            {
                End_User_ID = ID_Record.ID,
                Status = 2,
            });

            await IUsers_Repository_Update.Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP = dto.Server_IP,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                id = ID_Record.ID,
                name = $@"{dto.Name}#{user_public_id}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0",
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                login_type = "TWITCH",
            };
        }
        public async Task<User_Data_DTO> Create_Account_By_Discord(Complete_Discord_Registeration dto)
        {
            string user_public_id = await Generate_User_Public_ID();
            User_Secret_DTO user_secret = await Generate_User_Secret_ID();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                Public_ID = user_public_id,
                Secret_ID = user_secret.Encryption,
                Secret_Hash_ID = user_secret.Hash,
                Created_by = 0,
                Created_on = TimeStamp(),
                Updated_on = TimeStamp(),
                Updated_by = 0
            };

            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Discord_IDsTbl.AddAsync(new Discord_IDsTbl
            {
                End_User_ID = ID_Record.ID,
                Discord_ID = dto.Discord_ID,
                Discord_User_Name = dto.Discord_User_Name,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 9
            });

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                Name = $@"{dto.Discord_Name}",
                End_User_ID = ID_Record.ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Discord_Email_AddressTbl.AddAsync(new Discord_Email_AddressTbl
            {
                End_User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                End_User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Alignment(new Selected_App_Alignment
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Nav_Lock(new Selected_Navbar_Lock
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Text_Alignment(new Selected_App_Text_Alignment
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Theme(new Selected_Theme
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme
            });

            await IUsers_Repository_Update.Update_End_User_Account_Roles(new Account_Role
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Groups(new Account_Group
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });

            await IUsers_Repository_Update.Update_End_User_Account_Type(new Account_Types
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Grid_Type(new Selected_App_Grid_Type
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type
            });

            await IUsers_Repository_Update.Update_End_User_Selected_Status(new Selected_Status
            {
                End_User_ID = ID_Record.ID,
                Status = 2,
            });

            await IUsers_Repository_Update.Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP = dto.Server_IP,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                id = ID_Record.ID,
                name = $@"{dto.Name}#{user_public_id}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0",
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                login_type = "DISCORD",
            };
        }
        public async Task<string> Create_Pending_Email_Registration_Record(Pending_Email_Registration dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Language_Region = $"{dto.Language}-{dto.Region}",
                Email_Address = dto.Email_Address,
                Location = dto.Location,
                Client_time = dto.Client_Time_Parsed,
                Code = dto.Code,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                email_address = dto.Email_Address,
                code = dto.Code,
                language = dto.Language,
                region = dto.Region,
                created_on = TimeStamp(),
            });
        }
        public async Task<bool> Create_Contact_Us_Record(Contact_Us dto)
        {
            await _UsersDBC.Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                End_User_ID = dto.End_User_ID,
                Subject_Line = dto.Subject_line,
                Summary = dto.Summary,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });

            await _UsersDBC.SaveChangesAsync();

            return true;
        }
        public async Task<bool> Create_End_User_Status_Record(Selected_Status dto)
        {
            await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                End_User_ID = dto.End_User_ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Online = true,
                Updated_by = dto.End_User_ID
            });
            return true;
        }
        public async Task<bool> Create_Website_Bug_Record(Reported_Website_Bug dto)
        {
            await _UsersDBC.Reported_Website_BugTbl.AddAsync(new Reported_Website_BugTbl
            {
                End_User_ID = dto.End_User_ID,
                URL = dto.URL,
                Detail = dto.Detail,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_WebSocket_Log_Record(WebSocket_Chat_Permission dto)
        {
            try
            {
                await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Requested = true,
                    Approved = false,
                    Blocked = false
                });

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    updated_on = TimeStamp(),
                    updated_by = dto.End_User_ID,
                    updated_for = dto.User
                }));
            }
            catch
            {
                return "Server Error: WebSocket Log Record";
            }
        }
        public async Task<bool> Create_Discord_Bot_Bug_Record(Reported_Discord_Bot_Bug dto)
        {
            await _UsersDBC.Reported_Discord_Bot_BugTbl.AddAsync(new Reported_Discord_Bot_BugTbl
            {
                End_User_ID = dto.End_User_ID,
                Location = dto.Location,
                Detail = dto.Detail,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Comment_Box_Record(Comment_Box dto)
        {
            await _UsersDBC.Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                End_User_ID = dto.End_User_ID,
                Comment = dto.Comment,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Broken_Link_Record(Reported_Broken_Link dto)
        {
            await _UsersDBC.Reported_Broken_LinkTbl.AddAsync(new Reported_Broken_LinkTbl
            {
                End_User_ID = dto.End_User_ID,
                URL = dto.URL,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_Reported_User_Profile_Record(Reported_Profile dto)
        {
            Reported_ProfileTbl record = new Reported_ProfileTbl
            {
                End_User_ID = dto.End_User_ID,
                Reported_ID = dto.Reported_ID,
                Page_Title = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
                Reported_Reason = dto.Reported_reason,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = dto.End_User_ID
            };
            await _UsersDBC.Reported_ProfileTbl.AddAsync(record);
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                report_record_id = record.ID,
                reported_user_id = record.Reported_ID,
                created_on = record.Created_on,
                read_reported_user = IUsers_Repository_Read.Read_User_Data_By_ID(dto.Reported_ID).ToString(),
                read_reported_profile = IUsers_Repository_Read.Read_User_Profile_By_ID(dto.Reported_ID).ToString(),
            });
        }

        public async Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_Stamp_History dto)
        {
            try
            {
                await _UsersDBC.Login_Time_Stamp_HistoryTbl.AddAsync(new Login_Time_Stamp_HistoryTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Deleted = false,
                    Deleted_by = 0,
                    Deleted_on = 0,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID,
                    Created_by = dto.End_User_ID,
                    Login_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    login_on = TimeStamp()
                }));
            }
            catch
            {
                return Task.FromResult(JsonSerializer.Serialize("Login TS History Failed.")).Result;
            }
        }
    }
}
