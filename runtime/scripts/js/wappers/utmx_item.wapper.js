import { __UtmxItem ,__logger} from "__UTMX";

export class UtmxBaseItem extends __UtmxItem  
{
	set displayName(value) {
		this.DisplayName = value;
	}
	get displayName() {
		return this.DisplayName;
	}
	get slot() {
		return this.ItemSlot;
	}
	set slot(value) {	
		this.ItemSlot = value;
	}
	
	removeSelf()
	{
		this.RemoveSelf();
	} 
  
	onUsed()  
	{
	}
	  
	onDrop()  
	{  
	}  
  
	onInfo()  
	{  
	}  
}
