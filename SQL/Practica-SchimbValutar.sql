use master
go
if exists(select 'true' from sys.databases where name = 'Schimb_Valutar')
begin
	drop database Schimb_Valutar
end
go
create database Schimb_Valutar
go
alter authorization on database:: Schimb_Valutar to sa
go
use Schimb_Valutar
go
set dateformat dmy
go
create table Valuta(
	ID nvarchar(4) primary key,
	Cod nvarchar(3),
	Denumire nvarchar(100)
)
go
--delete from Valuta
--go
delete from Schimb
--go
select * from Valuta
go
select * from Schimb
go
--go
--drop table Tranzactie
--go
--drop table Schimb
--go
create table Schimb(
	ID nvarchar(7) primary key,
	ID_Valuta_Convertita nvarchar(4) foreign key references Valuta(ID) on delete cascade,
	Schimb money,
	ID_Valuta nvarchar(4)
)
go
select * from Clienti
go
create table Clienti(
	ID nvarchar(7) primary key,
	IDNP bigint check( len(IDNP) = 13 ) unique,
	Nume nvarchar(25),
	Prenume nvarchar(25),
	Adresa nvarchar(50),
	Telefon bigint check( len(Telefon) = 8 ) unique,
	Email nvarchar(50) unique
);
go
create table Tranzactie(
	ID nvarchar(7) primary key,
	ID_Client nvarchar(7) foreign key references Clienti(ID) on delete cascade,
	ID_Schimb nvarchar(7) foreign key references Schimb(ID) on delete cascade,
	Suma money,
	Data_tranz date
);
go
select * from Tranzactie