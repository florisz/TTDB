#light
module SimpleModel.Person.BerekenLeeftijd

open SimpleModel.Persoon
open System

let Execute(person: Persoon) =
    let timeSpan = ((DateTime.Today) - (person.GeboorteDatum)) 
    person.Leeftijd <- (timeSpan.Days /365)

    if person.Geslacht = "m" then
        person.Geslacht <- "man"
    else
        person.Geslacht <- "vrouw"

#light 
module SimpleModel.Persoon.BerekenLeeftijd 

open SimpleModel.Persoon 
open System 

let timeSpan = ((DateTime.Today) - (person.GeboorteDatum)) person.Leeftijd <- (timeSpan.Days /365) if person.Geslacht = "m" then person.Geslacht <- "man" else person.Geslacht <- "vrouw"        