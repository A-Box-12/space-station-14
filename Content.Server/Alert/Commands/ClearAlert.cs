using System;
using Content.Server.Administration;
using Content.Server.Commands;
using Content.Shared.Administration;
using Content.Shared.Alert;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Alert.Commands
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class ClearAlert : IConsoleCommand
    {
        public string Command => "clearalert";
        public string Description => "Clears an alert for a player, defaulting to current player";
        public string Help => "clearalert <alertType> <name or userID, omit for current player>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player as IPlayerSession;
            if (player?.AttachedEntity == null)
            {
                shell.WriteLine("You don't have an entity.");
                return;
            }

            var attachedEntity = player.AttachedEntity.Value;

            if (args.Length > 1)
            {
                var target = args[1];
                if (!CommandUtils.TryGetAttachedEntityByUsernameOrId(shell, target, player, out attachedEntity)) return;
            }

            if (!IoCManager.Resolve<IEntityManager>().TryGetComponent(attachedEntity, out ServerAlertsComponent? alertsComponent))
            {
                shell.WriteLine("user has no alerts component");
                return;
            }

            var alertType = args[0];
            var alertMgr = IoCManager.Resolve<AlertManager>();
            if (!alertMgr.TryGet(Enum.Parse<AlertType>(alertType), out var alert))
            {
                shell.WriteLine("unrecognized alertType " + alertType);
                return;
            }

            alertsComponent.ClearAlert(alert.AlertType);
        }
    }
}
