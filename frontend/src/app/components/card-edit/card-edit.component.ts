import { ElementRef, EventEmitter, HostListener, Input, OnInit, Output, ViewChild, ViewChildren } from '@angular/core';
import { Component } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Card } from 'src/app/models/card';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { CardService } from 'src/app/services/card.service';

@Component({
    selector: 'cards-details',
    templateUrl: 'card-edit.component.html',
    styleUrls: ['card-edit.component.css']
})
export class CardEditComponent implements OnInit{

    currentCard: Card;
    tableOwnerPseudo: string;
    addMembers: boolean = false;
    removeMembers: boolean = false;
    removeCard: boolean = false;
    hiddenTitle: boolean = false;
    cardTitleError: Observable<boolean> = of(false);
    @Input() tableUsers : User[];
    @Input() currentCardId : number;
    @Input() tableOwnerId : number;
    @Output() closeRequest = new EventEmitter<boolean>();
    @Output() cardChange = new EventEmitter<Card>();
    private textArea: ElementRef;
    @ViewChild("description", { static: false }) set content(content :ElementRef){
        if(content){
            this.textArea = content;
            this.textArea.nativeElement.focus();
        }
    }
    private titleInput: ElementRef;
    @ViewChild("title", { static: false }) set text(text :ElementRef){
        if(text){
            this.titleInput = text;
            this.titleInput.nativeElement.focus();
        }
    }
    @ViewChild("cardWindow") window: ElementRef; //removeCard
    @ViewChild("remove") removeCardWindow: ElementRef;

    constructor(
        private cardService: CardService,
        private authenticationService: AuthenticationService
    ) {
    }

    ngOnInit(): void {
        this.cardService.getCurrentCard(this.currentCardId).subscribe(c => {
            this.currentCard = new Card(c);
            if(this.ownerFollows()){
                this.tableOwnerPseudo = this.currentCard.usersIn.find(u => u.id == this.tableOwnerId).pseudo;
            }
        });
    }

    private imAdmin(): boolean{
        return this.currentUser.id == this.tableOwnerId;
    }

    private iFollow(): boolean{
        return this.currentCard.usersIn.includes(this.currentCard.usersIn.find(u => u.id == this.currentUser.id));
    }

    private ownerFollows(): boolean{
        return typeof(this.currentCard.usersIn.find(u => u.id == this.tableOwnerId)) !== 'undefined';
    }

    private closeCard(): void{
        this.closeRequest.emit(false);
    }

    private cardTitleChange() :void{
        this.cardChange.emit(this.currentCard);
    }

    private updateUsersList(user: User): void{
        this.cardService.addMember(this.currentCard.id, user.id).subscribe(u => {
            if(typeof(this.currentCard.usersIn.find(u => u.id == this.tableOwnerId)) !== 'undefined'){ 
                let tmp: User[] = new Array(this.currentCard.usersIn.length - 1);
                tmp = this.currentCard.usersIn.filter(u => u.id != this.tableOwnerId);
                var tmp2 = this.findLoc(user, tmp, 1);
                this.currentCard.usersIn.splice(tmp2+2, 0, user);
            } else if(user.id == this.tableOwnerId){
                this.currentCard.usersIn.splice(0, 0, user);
            } else {
                this.currentCard.usersIn.splice(this.findLoc(user, this.currentCard.usersIn)+1, 0, user);
            }
            this.cardChange.emit(this.currentCard);
        });
    }

    private removeUserFromList(user: User): void{
        this.cardService.removeMember(this.currentCard.id, user.id).subscribe();
        const index = this.currentCard.usersIn.findIndex(u => u.id == user.id);
        if(index > -1){
            this.currentCard.usersIn.splice(index, 1);
        }
        this.cardChange.emit(this.currentCard);
    }

    private findLoc(user: User, users: User[], st?: number, en?: number): number{
        st = st || 0; 
            en = en || users.length; 
            for (let i = 0; i < users.length; i++) { 
                if (users[i].pseudo > user.pseudo) 
                    return i - 1; 
            } 
        return en; 
    } 

    private join(): void{
        this.cardService.addMember(this.currentCardId, this.currentUser.id).subscribe(u => {
            this.currentCard.usersIn.unshift(this.currentUser);
            this.cardChange.emit(this.currentCard);
        });
    }

    private leave(): void{
        this.cardService.removeMember(this.currentCardId, this.currentUser.id).subscribe(u => {
            const index = this.currentCard.usersIn.findIndex(u => u.id == this.currentUser.id);
            this.currentCard.usersIn.splice(index, 1);
            this.cardChange.emit(this.currentCard);
        });
    }

    @HostListener('document:click', ['$event.target'])
    public onClick(targetElement) {
        if(!this.window.nativeElement.contains(targetElement) && !this.addMembers && !this.removeMembers) {
            this.closeCard();
        } if(!this.removeCardWindow.nativeElement.contains(targetElement)) {
            this.removeCard = false;
        } if(!this.titleInput.nativeElement.contains(targetElement)) { 
            this.hiddenTitle = false;
            this.cardTitleError = of(false);
        } 
    }

    get currentUser() {
        return this.authenticationService.currentUser;
    }

    private reducePseudo(pseudo: string): string {
        return pseudo.substr(0, 2).toUpperCase();
    }

    private saveContent(content: string): void{
        this.cardService.updateCardContent(this.currentCard.id, content).subscribe(c => {
            this.currentCard.content = content;
        });
    }

    private saveTitle(title: string): void{
        if(title != this.currentCard.title)
            this.cardService.updateCardTitle(this.currentCard.id, title).subscribe(c => {
                    this.currentCard.title = title;
                    this.cardTitleChange();
                    this.cardTitleError = of(false);
                    this.hiddenTitle = false;
                },
                error => {
                    this.cardTitleError = of(true);
            });
        else
            this.hiddenTitle = false;
    }

    private closeInput(request: boolean){
        this.addMembers = false;
        this.removeMembers = false;
    }

    delete(){
        this.cardService.deleteCard(this.currentCardId).subscribe(c => {
            this.currentCard = null;
            this.cardChange.emit(this.currentCard);
        });
    }

    private stopPropagation(event){
        event.stopPropagation();
    }

}