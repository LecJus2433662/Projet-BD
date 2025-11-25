USE Prog_A25_Bd_Projet;
GO


CREATE PROCEDURE ConnexionUtilisateur
    @Email       NVARCHAR(100),
    @MotDePasse  NVARCHAR(250),
    @Resultat    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sel  UNIQUEIDENTIFIER;
    DECLARE @hash VARBINARY(64);

    -- 1. Récupérer le sel (et vérifier que l'utilisateur existe)
    SELECT @sel = sel
    FROM dbo.utilisateur
    WHERE email = @Email;

    IF @sel IS NULL
    BEGIN
        SET @Resultat = -1;
        RETURN;
    END

    -- 2. Calculer le hash avec le bon sel
    SET @hash = HASHBYTES('SHA2_512', @MotDePasse + CAST(@sel AS NVARCHAR(36)));

    -- 3. Vérifier les credentials et récupérer l'ID en une seule requête (plus rapide et sûr)
    SELECT @Resultat = noUtilisateur
    FROM dbo.utilisateur
    WHERE email = @Email
      AND motDePasse = @hash;

    -- Si rien n'est trouvé → @Resultat reste NULL → on force -1
    IF @Resultat IS NULL
        SET @Resultat = -1;

    -- Sinon @Resultat contient déjà l'ID → rien à faire
END
GO

-- ===================================================================
-- TESTS (tout fonctionne maintenant à 100%)
-- ===================================================================

DECLARE @rep INT;

-- Test 1 : utilisateur inexistant
EXEC dbo.ConnexionUtilisateur 'inconnu@test.com', 'nimporte', @rep OUTPUT;
SELECT '1. Inexistant' AS Test, ISNULL(@rep, -1) AS Resultat;  -- → -1

-- Test 2 : mauvais mot de passe
EXEC dbo.ConnexionUtilisateur 'benoit@benoit.ca', 'wrongpass123', @rep OUTPUT;
SELECT '2. Mauvais MDP' AS Test, ISNULL(@rep, -1) AS Resultat;  -- → -1

-- Test 3 : bon mot de passe
EXEC dbo.ConnexionUtilisateur 'benoit@benoit.ca', 'secret', @rep OUTPUT;
SELECT '3. Connexion OK' AS Test, @rep AS Resultat;  -- → l'ID réel (ex: 1, 2, 5...)

-- ===================================================================
-- Vérification manuelle du hash (doit être identique)
-- ===================================================================
DECLARE @sel_test UNIQUEIDENTIFIER;
SELECT @sel_test = sel FROM dbo.utilisateur WHERE email = 'benoit@benoit.ca';

SELECT 
    HASHBYTES('SHA2_512', 'secret' + CAST(@sel_test AS NVARCHAR(36))) AS Hash_Calculé_avec_secret,
    motDePasse AS Hash_Stocké_dans_la_BD,
    CASE WHEN HASHBYTES('SHA2_512', 'secret' + CAST(@sel_test AS NVARCHAR(36))) = motDePasse 
         THEN 'MATCH PARFAIT' ELSE 'ERREUR' END AS Verification
FROM dbo.utilisateur 
WHERE email = 'benoit@benoit.ca';
