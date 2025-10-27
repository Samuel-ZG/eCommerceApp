using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AnadirTablasDePedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedido_Pedido_PedidoId",
                table: "DetallePedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedido_Productos_ProductoId",
                table: "DetallePedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_AspNetUsers_UsuarioId",
                table: "Pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pedido",
                table: "Pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallePedido",
                table: "DetallePedido");

            migrationBuilder.RenameTable(
                name: "Pedido",
                newName: "Pedidos");

            migrationBuilder.RenameTable(
                name: "DetallePedido",
                newName: "DetallePedidos");

            migrationBuilder.RenameIndex(
                name: "IX_Pedido_UsuarioId",
                table: "Pedidos",
                newName: "IX_Pedidos_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedido_ProductoId",
                table: "DetallePedidos",
                newName: "IX_DetallePedidos_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedido_PedidoId",
                table: "DetallePedidos",
                newName: "IX_DetallePedidos_PedidoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallePedidos",
                table: "DetallePedidos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedidos_Pedidos_PedidoId",
                table: "DetallePedidos",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedidos_Productos_ProductoId",
                table: "DetallePedidos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedidos_Pedidos_PedidoId",
                table: "DetallePedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedidos_Productos_ProductoId",
                table: "DetallePedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallePedidos",
                table: "DetallePedidos");

            migrationBuilder.RenameTable(
                name: "Pedidos",
                newName: "Pedido");

            migrationBuilder.RenameTable(
                name: "DetallePedidos",
                newName: "DetallePedido");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_UsuarioId",
                table: "Pedido",
                newName: "IX_Pedido_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedidos_ProductoId",
                table: "DetallePedido",
                newName: "IX_DetallePedido_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedidos_PedidoId",
                table: "DetallePedido",
                newName: "IX_DetallePedido_PedidoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pedido",
                table: "Pedido",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallePedido",
                table: "DetallePedido",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedido_Pedido_PedidoId",
                table: "DetallePedido",
                column: "PedidoId",
                principalTable: "Pedido",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedido_Productos_ProductoId",
                table: "DetallePedido",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_AspNetUsers_UsuarioId",
                table: "Pedido",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
