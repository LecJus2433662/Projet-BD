USE Prog_A25_BD_Projet;
Go
CREATE PROCEDURE ConnexionUtilisateur
    @Email NVARCHAR(100),
    @MotDePasse NVARCHAR(250),
	@Resultat INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id INT;
    DECLARE @sel UNIQUEIDENTIFIER;
    DECLARE @hash VARBINARY(64);
	

    SELECT @sel = sel FROM utilisateur WHERE email = @Email;

    IF @sel IS NULL
    BEGIN
        SET @Resultat = -1; 
        RETURN;
    END

    SET @hash = HASHBYTES('SHA2_256', @MotDePasse + CAST(@sel AS NVARCHAR(36)));

    SELECT @Id = noUtilisateur
    FROM utilisateur
    WHERE email = @Email AND motDePasse = @hash;

    IF @Id IS NULL
        SET @Resultat = -1; 
    ELSE
        SET @Resultat = @Id;
END
GO
DECLARE @reponse INT;

exec dbo.ConnexionUtilisateur
	@Email = 'benoit@benoit.ca',
	@MotDePasse = 'secret',
	@Resulatat = @reponse OUTPUT;

SELECT @reponse AS CodeReturn;

