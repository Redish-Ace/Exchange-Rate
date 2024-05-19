use Schimb_Valutar
go
--Vedere care afiseaza codul si denumirea valutei convertire, suma schimbului si codul si denumirea valutei in care se converteste
drop view if exists getRate
go
create view getRate as
	select s.ID, vc.Cod + ': ' + vc.Denumire as Valuta_Convertita, s.Schimb, v.Cod + ': ' + v.Denumire as Valuta
		from Schimb s
		join Valuta vc on vc.ID = s.ID_Valuta_Convertita
		join Valuta v on v.ID = s.ID_Valuta
go
select * from getRate
select * from Valuta
select * from Schimb

--Vedere care afiseaza tranzactii
drop view if exists tranzactii
go
create view tranzactii as
	select t.ID, c.Nume + ' ' + c.Prenume as Client, r.Valuta_Convertita, r.Schimb, r.Valuta, t.Suma, (t.Suma * r.Schimb - t.Suma * r.Schimb * 0.02) as Suma_Finala, (t.Suma * r.Schimb * 0.02) as Comision, Data_tranz
		from Tranzactie t
		join Clienti c on c.ID = t.ID_Client
		join getRate r on r.ID = t.ID_Schimb
go
select * from tranzactii
--Vedere care afiseaza tranzactii care au fost facute azi
drop view if exists tranzactiiAzi
go
create view tranzactiiAzi as
	select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_Finala * r.Schimb as Suma_finala_lei, Comision * r.Schimb as Comision_lei, Data_tranz
		from tranzactii t
		join getRate r on t.Valuta=r.Valuta_Convertita
			where r.Valuta like 'MDL%' and Data_tranz = convert(varchar, GETDATE(), 23)
go
select * from tranzactiiAzi
--Vederea care afiseaza valuta cea mai solicitata
drop view if exists valutaSolicitata
go
create view valutaSolicitata as
select ID, Cod, Denumire
from Valuta v
where exists (
    select ID_Valuta
    from Schimb s
    where s.ID_Valuta = v.ID
    and s.ID in (
        select top 1 ID_Schimb
        from Tranzactie
        group by ID_Schimb
        order by count(*) desc
    )
)
go
select * from valutaSolicitata
--Vedere care afiseaza cea mai avantajoasa tranzactie in lei
drop view if exists tranzactieAvantaj
go
create view tranzactieAvantaj as
    select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_Finala * r.Schimb as Suma_finala_lei, Comision * r.Schimb as Comision_lei, Data_tranz
		from tranzactii t
		join getRate r on t.Valuta=r.Valuta_Convertita
			where r.Valuta like 'MDL%'
go
select top 1 * from tranzactieAvantaj
	order by Suma_finala_lei desc
--Vedere care afiseaza comisionul total
drop view if exists comisionTotal
go
create view comisionTotal as
    select sum(Comision_lei) as Comision_Total
		from tranzactieAvantaj
go
select * from comisionTotal
--Crearea unei functii care afiseaza tranzactiile din data introdusa in ordine descrescatoare sumei
go
drop function if exists tranzactiiData
go
create function tranzactiiData(@date date)
returns table
as
	return (select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_finala_lei, Comision_lei, Data_tranz
		from tranzactieAvantaj t
			where Data_tranz = @date)
go
select * from tranzactiiData('2024-05-14')
	order by Suma_finala_lei desc
--Crearea unei proceduri ce insereaza in tabelul Tranzactie datele introduse la executarea procedurii si introducerea datii tranzactiei prin functia getdate() prin update
go
drop procedure if exists insertTranzactii
go
create procedure insertTranzactii
@id nvarchar(7),
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@suma money
as
begin
	if (@id is not null) and (@id_client is not null) and (@id_schimb is not null) and (@suma != 0)
	begin
		insert into Tranzactie (ID, ID_Client, ID_Schimb, Suma) 
			values(@id, @id_client, @id_schimb, @suma)

		update Tranzactie
			set Data_tranz = getdate()
				where ID = @id
	end;
end
exec insertTranzactii 't000002', 'c000001', 's015', 200
--Crearea unei proceduri ce modifica datele din tabelul Tranzactie
go
drop procedure if exists updateTranzactii
go
create procedure updateTranzactii
@id nvarchar(7),
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@suma money,
@date date
as
begin
	if (@id is not null) 
	begin
		if (@id_client is not null) 
		begin
			update Tranzactie
				set ID_Client = @id_client
					where ID = @id
		end;
		if (@id_schimb is not null) 
		begin
			update Tranzactie
				set ID_Schimb = @id_schimb
					where ID = @id
		end;
		if (@suma != 0) 
		begin
			update Tranzactie
				set Suma = @suma
					where ID = @id
		end;
		if (@date is not null) 
		begin
			update Tranzactie
				set Data_tranz = @date
					where ID = @id
		end;
	end;
end
--Crearea unei fucntii care afiseaza id-ul tranzactiei dupa client, schimb, suma si data
go
drop function if exists getTranzactii
go
create function getTranzactii(
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@suma money)
returns nvarchar(7)
as
begin
	return (select ID from Tranzactie where (ID_Client = @id_client) or (ID_Schimb = @id_schimb) or (Suma = @suma))
end
go
--Crearea unei proceduri care insereaza date despre clienti
go
drop procedure if exists insertClienti
go
create procedure insertClienti
@id nvarchar(7),
@idnp bigint,
@nume nvarchar(25),
@prenume nvarchar(25),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50)
as
begin
	if (@id is not null) and (@idnp != 0) and (@nume is not null) and (@prenume is not null) and (@adresa is not null) and (@telefon != 0) and (@email is not null)
	begin
		insert into Clienti 
			values(@id, @idnp, @nume, @prenume, @adresa, @telefon, @email)
	end;
end

select * from Clienti
--Crearea unei proceduri ce modifica datele din tabelul Tranzactie
go
drop procedure if exists updateTranzactii
go
create procedure updateTranzactii
@id nvarchar(7),
@nume nvarchar(50),
@prenume nvarchar(50),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50)
as
begin
	if (@id is not null) 
	begin
		if (@nume is not null) 
		begin
			update Clienti
				set Nume = @nume
					where ID = @id
		end;
		if (@prenume is not null) 
		begin
			update Clienti
				set Prenume = @prenume
					where ID = @id
		end;
		if (@adresa is not null) 
		begin
			update Clienti
				set Adresa = @adresa
					where ID = @id
		end;
		if (@telefon != 0) 
		begin
			update Clienti
				set Telefon = @telefon
					where ID = @id
		end;
		if (@email is not null) 
		begin
			update Clienti
				set Email = @email
					where ID = @id
		end;
	end;
end
--Crearea unei proceduri ce sterge datele din tabelul Clienti
go
drop function if exists getClient
go
create function getClient(
@idnp bigint,
@nume nvarchar(50),
@prenume nvarchar(50),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50))
returns nvarchar(7)
as
begin
	return (select ID from Clienti where (IDNP = @idnp) or (Nume = @nume) or (Prenume = @prenume) or (Adresa = @adresa) or (Telefon = @telefon) or (Email = @email))
end