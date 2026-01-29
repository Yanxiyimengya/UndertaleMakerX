import { __UtmxItem } from "__UTMX";

export class UtmxBaseItem extends __UtmxItem  
{
	set name(value) {
		this.DisplayName = value;
	}
	get name() {
		return this.DisplayName;
	}

	get slot() {
		return this.Slot;
	}
	set slot(value) {	
		this.Slot = value;
	}

	constructor() {
		super();
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
