#ifndef __CDK__CSListBox__
#define __CDK__CSListBox__

#include "CSScrollPane.h"

class CSListBox : public CSScrollPane {
public:
	CSHandler<CSListBox*, CSLayer*> OnSelect;
private:
    CSLayer* _selectedItem = NULL;
    bool _selectionChanged = false;
    bool _autoFocus = true;
public:
    CSListBox();
protected:
    virtual ~CSListBox() = default;
public:
    static inline CSListBox* listBox() {
        return autorelease(new CSListBox());
    }
protected:
    virtual void onSelect(CSLayer* next);
public:
    inline CSLayer* selectedItem() const {
        return _selectedItem;
    }
    void setSelectedItem(CSLayer* selectedItem);
    int selectedIndex() const;
    void setSelectedIndex(int selectedIndex);
    inline bool selectionChanged() const {
        return _selectionChanged;
    }
    inline bool autoFocus() const {
        return _autoFocus;
    }
    inline void setAutoFocus(bool autoFocus) {
        _autoFocus = autoFocus;
    }
    virtual bool addLayer(CSLayer* layer) override;
    virtual bool insertLayer(int index, CSLayer* layer) override;
protected:
    virtual void detach(CSLayer* layer) override;
    virtual void submitLayout() override;
    virtual void onTouchesMoved() override;
private:
    void onTouchesBeganItem(CSLayer* layer);
    void changeSelectedItem(CSLayer* selectedItem);
};

#endif
