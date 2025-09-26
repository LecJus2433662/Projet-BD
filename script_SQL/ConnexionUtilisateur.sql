CREATE PROCEDURE Connexionutilisateur
    @Email NVARCHAR(100),
    @MotDePasse NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;
    DECLARE @sel UNIQUEIDENTIFIER;
    DECLARE @hash VARBINARY(64);

    SELECT @sel = sel FROM utilisateur WHERE email = @Email;

    IF @sel IS NULL
    BEGIN
        SELECT -1 AS Resultat; 
        RETURN;
    END

    SET @hash = HASHBYTES('SHA2_256', @MotDePasse + CAST(@sel AS NVARCHAR(36)));

    SELECT @Id = noUtilisateur
    FROM utilisateur
    WHERE email = @Email AND motDePasse = @hash;

    IF @Id IS NULL
        SELECT -1 AS Resultat; 
    ELSE
        SELECT @Id AS Resultat;
END
GO
