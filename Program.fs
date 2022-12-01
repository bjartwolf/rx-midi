open NAudio.Midi
open System
open FSharp.Control.Reactive

let midiIn = new MidiIn(0)
let notes = midiIn.MessageReceived |> Observable.filter (fun (x: MidiInMessageEventArgs) -> x.MidiEvent :? NoteEvent)
                                   |> Observable.map (fun (x: MidiInMessageEventArgs) -> x.MidiEvent :?> NoteEvent) 

let notesOn = notes |> Observable.filter (fun n -> n.CommandCode = MidiCommandCode.NoteOn)
let output = notesOn |> Observable.bufferCount 3
let arpeggio = output |> Observable.switchMap (fun x -> Observable.ofSeq(x) |> Observable.zip (Observable.interval (TimeSpan.FromMilliseconds 150)))
arpeggio |> Observable.subscribe (fun (notes) -> printfn "%A" notes) |> ignore
midiIn.Start()
Console.ReadKey() |> ignore
// https://twitter.com/bjartnes/status/1581229189675778048/photo/1